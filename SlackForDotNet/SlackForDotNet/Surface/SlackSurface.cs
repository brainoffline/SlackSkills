﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using SlackForDotNet.WebApiContracts;

namespace SlackForDotNet.Surface
{
    public abstract class SlackSurface
    {
        private                 string?                    _lastUpdated;
        
        protected static readonly Dictionary< string, View > Views = new ();
        private static            int                        _ids   = 1;
        protected readonly        ISlackApp                  App;
        
        protected                 string?                    TriggerId { get; private set; }

        public    string?        ExternalId  { get; set; }
        public    int            UniqueId    { get; set; }
        public    string         UniqueValue { get; set; }
        public    List< Layout > Layouts     { get; set; } = new ();
        public    PlainText      Title       { get; set; } = "";
        public    User?          User        { get; private set; }
        protected Team?          Team        { get; private set; }

        public string? ViewId     { get; set; }
        public string? AppId      { get; set; }
        public string? BotId      { get; set; }
        public string? TeamId     { get; set; }
        public string? CallbackId { get; set; }
        public string? Hash       { get; set; }
        public string? RootViewId { get; set; }

        protected SlackSurface( [NotNull] ISlackApp app )
        {
            App = app;
            UniqueId = Interlocked.Increment( ref _ids );
            UniqueValue = $"_{UniqueId}_";
        }
        
        private void UpdateState(object? values)
        {
            if (values == null) return;

            var j = (JObject)values;
            var state = j.ToObject<Dictionary<string, Dictionary<string, Surface.Element>>>();

            if (state == null)
                return;
            foreach (var pair in state)
            {
                var layout = Layouts.FirstOrDefault(l => l.block_id == pair.Key);
                var blockId = pair.Key;
                foreach (var actionPair in pair.Value)
                {
                    var actionId = actionPair.Key;
                    var actionValue = actionPair.Value;
                    var type = actionValue.type;

                    var element = FindElement(type, blockId, actionId);
                    switch (element)
                    {
                        case TextInputElement textInput:
                            var tia = (TextInputElement)actionValue;
                            textInput.value = tia.value ?? "";
                            break;
                        case CheckboxesElement checkboxes:
                            var ce = (CheckboxesElement)actionValue;
                            checkboxes.selected_options = ce.selected_options;
                            break;
                        case RadioButtonsElement radioButtons:
                            var re = (RadioButtonsElement)actionValue;
                            radioButtons.selected_option = re.selected_option;
                            break;
                        case DatePickerElement datePicker:
                            var dpe = (DatePickerElement)actionValue;
                            datePicker.selected_date = dpe.selected_date;
                            break;
                        case TimePickerElement timePicker:
                            var tpe = (TimePickerElement)actionValue;
                            timePicker.selected_time = tpe.selected_time;
                            break;
                    }
                }
            }
        
        }

        public void Process( BlockSuggestion blockSuggestion, string envelopeId )
        {
            TriggerId = blockSuggestion.trigger_id ?? "";
            User      = blockSuggestion.user;
            Team      = blockSuggestion.team;

            UpdateState(blockSuggestion.state?.values ?? blockSuggestion.view?.state?.values);

            var element = FindElement("", blockSuggestion.block_id, blockSuggestion.action_id);
            switch (element)
            {
                case SelectExternalElement select:
                    var options = select.Suggestions?.Invoke( blockSuggestion.value );
                    if (options != null)
                        App.Push( new AcknowledgeOptions( envelopeId, options )  );
                    break;
            }
        }

        public void Process(BlockActions blockActions)
        {
            TriggerId = blockActions.trigger_id ?? "";
            User      = blockActions.user;
            Team      = blockActions.team;
            var containerType = blockActions.container?.type ?? "";
            var view          = blockActions.view;
            var responseUrl   = blockActions.response_url;
            
            UpdateState( blockActions.state?.values ?? blockActions.view?.state?.values );
            
            if (blockActions.actions != null)
            {
                foreach (var slackAction in blockActions.actions)
                {
                    var type     = slackAction.type;
                    var blockId  = slackAction.block_id;
                    var actionId = slackAction.action_id;
                    var actionTs = slackAction.action_ts;
                    
                    // TODO: Trigger actions
                    var element = FindElement( type, blockId, actionId );

                    switch (element)
                    {
                        case ButtonElement btn:
                            btn.RaiseClicked();
                            break;
                        case TextInputElement textInput:
                            var tia = (TextInputAction)slackAction;
                            textInput.value = tia.value;
                            textInput.RaiseTextUpdatedEvent( tia.value ?? "" );
                            break;
                        case OverflowMenuElement overflowMenu:
                            var ome = (OverflowMenuAction)slackAction;
                            overflowMenu.selected_option = ome.selected_option;
                            if (!string.IsNullOrEmpty(ome.selected_option?.value))
                                overflowMenu.RaiseClicked(ome.selected_option.value);
                            break;
                        case SelectElement select:
                            var sea = (SelectAction)slackAction;
                            select.selected_option = sea.selected_option;
                            if (!string.IsNullOrEmpty(sea.selected_option?.value))
                                select.RaiseClicked(sea.selected_option.value);
                            break;
                    }
                }
            }
        }

        private Element? FindElement( string type, string blockId, string actionId )
        {
            foreach (var layout in Layouts.Where( layout => layout.block_id == blockId ))
            {
                switch (layout)
                {
                case ContextLayout ctx:
                {
                    var element = ctx.elements.FirstOrDefault( e => e.action_id == actionId );

                    if (element != null)
                        return element;

                    break;
                }
                case SectionLayout section:
                {
                    var element = ( section.accessory != null && section.accessory.action_id == actionId )
                                      ? section.accessory
                                      : default;
                    if (element != null)
                        return element;

                    break;
                }
                case InputLayout input:
                {
                    var element = ( input.element != null && input.element.action_id == actionId )
                                      ? input.element
                                      : default;

                    if (element != null)
                        return element;

                    break;
                }
                case ActionLayout action:
                {
                    var element = action.elements.FirstOrDefault(e => e.action_id == actionId);

                    if (element != null)
                        return element;

                    break;
                }
                }
            }

            return default;
        }

        public virtual Task Process(AppHomeOpened msg) { return Task.FromResult(true); }
        
        protected void RaiseError(ViewResponse response)
        {
            var sb = new StringBuilder();
            if (response.error != null)
                sb.AppendLine($"Error {response.error}");
            if (response.warning != null)
                sb.AppendLine($"Warning {response.warning}");
            if (response.detail != null)
                sb.AppendLine($"Detail {response.detail}");
            if (response.provided != null)
                sb.AppendLine($"Provided {response.provided}");
            if (response.needed != null)
                sb.AppendLine($"Needed {response.needed}");
            var messages = response.response_metadata?.messages;
            if (messages != null)
            {
                foreach (var message in messages)
                    sb.AppendLine($"\t{message}");
            }

            var logMsg = sb.ToString();
            App.Logger?.LogError(logMsg);
            RaiseError(logMsg);
        }

        protected void RaiseError(string msg)
        {
            // TODO: Allow program to whatever
        }
    }
}