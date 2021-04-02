using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using SlackSkills.WebApiContracts;

namespace SlackSkills.Surface
{
    public class SlackSurface
    {
        protected static readonly Dictionary< string, View > Views = new ();
        private static            int                        _ids  = 1;
        protected readonly        ISlackApp                  SlackApp;
        public                    string?                    channelId;
        public                    string?                    userId;
        public                    string?                    ts;
        public                    MessageBase?               message;

        public string? TriggerId { get; private set; }
        public string? EnvelopeId { get; private set; }
        public View?   View      { get; set; }

        public  int                    UniqueId         { get; set; }
        public  string                 UniqueValue      { get; set; }
        private List< Layout >         Layouts          { get; set; } = new();
        public  PlainText              Title            { get; set; } = "";
        public  ViewResponse.MetaData? ResponseMetaData { get; set; }

        public Action< ViewSubmission >? Submitted = null;
        public Action< ViewClosed >?     Closed    = null;

        public SlackSurface(  ISlackApp app )
        {
            SlackApp = app;
            UniqueId = Interlocked.Increment( ref _ids );
            UniqueValue = $"_{UniqueId}_";
        }

        public bool HasLayouts =>
            Layouts.Count > 0;

        public virtual List< Layout > BuildLayouts()
        {
            if (Layouts.Count == 0 && View?.blocks != null)
                Add(View.blocks);
            if (View != null)
            {
                View.blocks      =   Layouts;
                View.callback_id ??= UniqueValue;

                if (View is ModalView modal)
                {
                    modal.title = Title;
                }
            }
            var callbackId = View?.callback_id ?? UniqueValue;

            foreach (var layout in Layouts)
                layout.block_id ??= callbackId + new Random().NextDouble();

            return Layouts;
        }

        public SlackSurface ClearLayouts()
        {
            Layouts.Clear();

            return this;
        }

        public SlackSurface Add(Layout layout)
        {
            if (layout.block_id != null)
            {
                var old = FindLayout( layout.block_id );
                if (old != null)
                    Layouts.Remove( old );
            }

            Layouts.Add(layout);

            return this;
        }
        public SlackSurface Add(IEnumerable<Layout> layouts)
        {
            foreach (var layout in layouts)
                Add(layout);

            return this;
        }

        public void UpdateStateFrom( View view )
        {
            if (View == null)
                View = view;
            else
            {
                View.id           = view.id;
                View.app_id       = view.app_id;
                View.bot_id       = view.bot_id;
                View.team_id      = view.team_id;
                View.callback_id  = view.callback_id;
                View.external_id  = view.external_id;
                View.hash         = view.hash;
                View.root_view_id = view.root_view_id;
            }

            if (view.blocks != null && view.blocks.Count == Layouts.Count)
            {
                // Update block_id's
                for (int i = 0; i < view.blocks.Count; i++)
                {
                    var updatedLayout = view.blocks[i];
                    var layout    = Layouts[i];
                    if (string.IsNullOrEmpty(layout.block_id))
                        layout.block_id = updatedLayout.block_id;
                }
            }

            UpdateState( view.state?.values );
        }

        private  void UpdateState(object? values)
        {
            if (values == null) return;

            var j = (JObject)values;
            var state = j.ToObject<Dictionary<string, Dictionary<string, IElement>>>();

            if (state == null)
                return;
            foreach (var pair in state)
            {
                var blockId = pair.Key;
                foreach (var actionPair in pair.Value)
                {
                    var actionId = actionPair.Key;
                    var actionValue = actionPair.Value;

                    var element = FindElement(blockId, actionId);
                    switch (element)
                    {
                        case TextInputElement e:
                            e.value = ((TextInputElement)actionValue).value ?? "";
                            break;
                        case CheckboxesElement e:
                            e.selected_options = ((CheckboxesElement)actionValue).selected_options;
                            break;
                        case RadioButtonsElement e:
                            e.selected_option = ((RadioButtonsElement)actionValue).selected_option;
                            break;
                        case DatePickerElement e:
                            e.selected_date = ((DatePickerElement)actionValue).selected_date;
                            break;
                        case TimePickerElement e:
                            e.selected_time = ((TimePickerElement)actionValue).selected_time;
                            break;
                        case OverflowMenuElement e:
                            e.selected_option = ((OverflowMenuElement)actionValue).selected_option;
                            break;
                        case SelectElement e:
                            e.selected_option = ((SelectElement)actionValue).selected_option;
                            break;
                        case SelectExternalElement e:
                            e.selected_option = ((SelectExternalElement)actionValue).selected_option;
                            break;
                        case UsersSelectElement e:
                            e.selected_user = ( (UsersSelectElement)actionValue ).selected_user;
                            break;
                        case ConversationSelectElement e:
                            e.selected_conversation = ((ConversationSelectElement)actionValue).selected_conversation;
                            break;
                        case ChannelSelectElement e:
                            e.selected_channel = ((ChannelSelectElement)actionValue).selected_channel;
                            break;
                        case MultiSelectElement e:
                            e.selected_options = ((MultiSelectElement)actionValue).selected_options;
                            break;
                        case MultiSelectExternalElement e:
                            e.selected_options = ((MultiSelectExternalElement)actionValue).selected_options;
                            break;
                        case MultiUserSelectElement e:
                            e.selected_users = ((MultiUserSelectElement)actionValue).selected_users;
                            break;
                        case MultiConversationSelectElement e:
                            e.selected_conversations = ((MultiConversationSelectElement)actionValue).selected_conversations;
                            break;
                        case MultiChannelSelectElement e:
                            e.selected_channels = ((MultiChannelSelectElement)actionValue).selected_channels;
                            break;
                        default:
                            break;
                    }
                }
            }
        
        }

        public virtual void Process( ViewSubmission viewSubmission, string envelopeId )
        {
            TriggerId = viewSubmission.trigger_id ?? "";

            UpdateState(viewSubmission.view?.state?.values);
            EnvelopeId = envelopeId;

            Submitted?.Invoke( viewSubmission );

            var result = new ViewSubmissionResult();
            SlackApp.Push( new AcknowledgeResponse<ViewSubmissionResult>{ envelope_id = envelopeId, payload = result } );
        }

        public virtual void Process( ViewClosed viewClosed )
        {
            TriggerId = null;

            UpdateState(viewClosed.view?.state?.values);

            Closed?.Invoke(viewClosed);
        }

        public virtual void Process( BlockSuggestion blockSuggestion, string envelopeId )
        {
            TriggerId = blockSuggestion.trigger_id;

            UpdateState(blockSuggestion.state?.values ?? blockSuggestion.view?.state?.values);

            var element = FindElement(blockSuggestion.block_id, blockSuggestion.action_id);
            switch (element)
            {
            case SelectExternalElement select:
                {
                    var options = select.Suggestions?.Invoke( blockSuggestion.value );
                    if (options != null)
                        SlackApp.Push( new AcknowledgeResponse< Options > { envelope_id = envelopeId, payload = options } );
                }
                break;
            case MultiSelectExternalElement multiSelect:
                {
                    var options = multiSelect.Suggestions?.Invoke( blockSuggestion.value );
                    if (options != null)
                        SlackApp.Push( new AcknowledgeResponse< Options > { envelope_id = envelopeId, payload = options } );
                }
                break;
            }
        }

        public virtual void Process(BlockActions blockActions)
        {
            TriggerId = blockActions.trigger_id ?? "";
            
            UpdateState( blockActions.state?.values ?? blockActions.view?.state?.values );
            
            if (blockActions.actions != null)
            {
                foreach (var slackAction in blockActions.actions)
                {
                    var blockId  = slackAction.block_id;
                    var actionId = slackAction.action_id;
                    
                    var element = FindElement( blockId, actionId );

                    switch (element)
                    {
                        case ButtonElement btn:
                            btn.RaiseClicked(this, blockActions);
                            break;
                        case TextInputElement textInput:
                            var tia = (TextInputAction)slackAction;
                            textInput.value = tia.value;
                            textInput.RaiseTextUpdatedEvent(this, tia);
                            break;
                        case OverflowMenuElement overflowMenu:
                            var oma = (OverflowMenuAction)slackAction;
                            overflowMenu.selected_option = oma.selected_option;
                            if (!string.IsNullOrEmpty(oma.selected_option?.value))
                                overflowMenu.RaiseClicked(this, oma);
                            break;
                        case SelectElement select:
                            var sa = (SelectAction)slackAction;
                            select.selected_option = sa.selected_option;
                            if (!string.IsNullOrEmpty(sa.selected_option?.value))
                                select.RaiseClicked(this, sa);
                            break;
                        case SelectExternalElement selectExternal:
                            var sea = (SelectExternalAction)slackAction;
                            selectExternal.selected_option = sea.selected_option;
                            if (!string.IsNullOrEmpty(sea.selected_option?.value))
                                selectExternal.RaiseClicked(this, sea);
                            break;
                        case UsersSelectElement userSelect:
                            var usa = (UserSelectAction)slackAction;
                            userSelect.selected_user = usa.selected_user;
                            if (!string.IsNullOrEmpty(usa.selected_user))
                                userSelect.RaiseClicked(this, usa);
                            break;
                        case ConversationSelectElement conversationSelect:
                            var csa = (ConversationSelectAction)slackAction;
                            conversationSelect.selected_conversation = csa.selected_conversation;
                            if (!string.IsNullOrEmpty(csa.selected_conversation))
                                conversationSelect.RaiseClicked(this, csa);
                            break;
                        case ChannelSelectElement channelSelect:
                            var chsa = (ChannelSelectAction)slackAction;
                            channelSelect.selected_channel = chsa.selected_channel;
                            if (!string.IsNullOrEmpty(chsa.selected_channel))
                                channelSelect.RaiseClicked(this, chsa);
                            break;
                        case MultiSelectElement multiSelect:
                            var msa = (MultiSelectAction)slackAction;
                            multiSelect.selected_options = msa.selected_options;
                            if (msa.selected_options != null)
                                multiSelect.RaiseClicked(this, msa);
                            break;
                        case MultiSelectExternalElement multiSelectExternal:
                            var msea = (MultiSelectExternalAction)slackAction;
                            multiSelectExternal.selected_options = msea.selected_options;
                            if (msea.selected_options != null)
                                multiSelectExternal.RaiseClicked(this, msea);
                            break;
                        case MultiUserSelectElement multiUserSelect:
                            var musa = (MultiUserSelectAction)slackAction;
                            multiUserSelect.selected_users = musa.selected_users;
                            if (musa.selected_users != null)
                                multiUserSelect.RaiseClicked(this, musa);
                            break;
                        case MultiConversationSelectElement multiConversationSelect:
                            var mcsa = (MultiConversationSelectAction)slackAction;
                            multiConversationSelect.selected_conversations = mcsa.selected_conversations;
                            if (mcsa.selected_conversations != null)
                                multiConversationSelect.RaiseClicked(this, mcsa);
                            break;
                        case MultiChannelSelectElement multiChannelSelect:
                            var mchsa = (MultiChannelSelectAction)slackAction;
                            multiChannelSelect.selected_channels = mchsa.selected_channels;
                            if (mchsa.selected_channels != null)
                                multiChannelSelect.RaiseClicked(this, mchsa);
                            break;
                    }
                }
            }
        }

        public Layout? FindLayout( string blockId ) =>
            Layouts.FirstOrDefault( layout => layout.block_id == blockId );

        public IElement? FindElement( string blockId, string actionId )
        {
            foreach (var layout in Layouts.Where( layout => layout.block_id == blockId ))
            {
                switch (layout)
                {
                case ContextLayout ctx:
                {
                    var element = ctx.elements.FirstOrDefault( e => e.action_id == actionId );

                    if (element != default)
                        return element;

                    break;
                }
                case SectionLayout section:
                {
                    var element = ( section.accessory != null && section.accessory.action_id == actionId )
                                      ? section.accessory
                                      : default;
                    if (element != default)
                        return element;

                    break;
                }
                case InputLayout input:
                {
                    var element = ( input.element != null && input.element.action_id == actionId )
                                      ? input.element
                                      : default;

                    if (element != default)
                        return element;

                    break;
                }
                case ActionsLayout action:
                {
                    var element = action.elements.FirstOrDefault(e => e.action_id == actionId);

                    if (element != default)
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
            SlackApp.Logger?.LogError(logMsg);
            RaiseError(logMsg);
        }

        protected virtual void RaiseError(string msg)
        {
            // TODO: Allow program to whatever
        }
    }
}
