using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills.WebApiContracts
{
    [SlackMessage( "workflows.stepCompleted",
                     apiType: Msg.FormEncoded | Msg.Json | Msg.BotToken | Msg.UserToken,
                     scopes: new[] { "workflow.steps:execute" })]
    public class WorkflowsStepCompleted : SlackMessage
    {
        public string                     workflow_step_execute_id { get; set; }
        public Dictionary<string,string>? outputs                  { get; set; }
    }

    [SlackMessage( "workflows.stepFailed",
                     apiType: Msg.FormEncoded | Msg.Json | Msg.BotToken | Msg.UserToken,
                     scopes: new[] { "workflow.steps:execute" })]
    public class WorkflowsStepFailed : SlackMessage
    {
        public string                      workflow_step_execute_id { get; set; }
        public Dictionary<string, string>? outputs                  { get; set; }
        public object                      error                    { get; set; }
    }

    [SlackMessage( "workflows.updateStep",
                     apiType: Msg.FormEncoded | Msg.Json | Msg.BotToken | Msg.UserToken,
                     scopes: new[] { "workflow.steps:execute" })]
    public class WorkflowsUpdateStep : SlackMessage
    {
        public string                      workflow_step_edit_id { get; set; }
        public Dictionary<string, string>? inputs                { get; set; }
        public Dictionary<string, string>? outputs               { get; set; }
        public string?                     step_image_url        { get; set; }
        public string?                     name                  { get; set; }

    }
}
