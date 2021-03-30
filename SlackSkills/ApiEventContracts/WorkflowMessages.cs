using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills
{
    [SlackMessage( "workflow_deleted")]
    public class WorkflowDeleted : SlackMessage
    {
        public string  workflow_id                  { get; set; }
        public object? workflow_draft_configuration { get; set; }
        public string  event_ts                     { get; set; }
    }

    [SlackMessage( "workflow_published")]
    public class WorkflowPublished : SlackMessage
    {
        public string  workflow_id                      { get; set; }
        public object? workflow_published_configuration { get; set; }
        public string  event_ts                         { get; set; }
    }

    [SlackMessage( "workflow_step_deleted")]
    public class WorkflowStepDeleted : SlackMessage
    {
        public string  workflow_id                      { get; set; }
        public object? workflow_draft_configuration     { get; set; }
        public object? workflow_published_configuration { get; set; }
        public string  event_ts                         { get; set; }
    }

    [SlackMessage( "workflow_step_executed")]
    public class WorkflowStepExecuted : SlackMessage
    {
        public string        callback_id   { get; set; }
        public WorkflowStep? workflow_step { get; set; }
    }

    [SlackMessage( "workflow_unpublished")]
    public class WorkflowUnPublished : SlackMessage
    {
        public string  workflow_id                  { get; set; }
        public object? workflow_draft_configuration { get; set; }
        public string  event_ts                     { get; set; }
    }



    public class WorkflowStep
    {
        public string  workflow_step_execute_id { get; set; }
        public object? inputs                   { get; set; }
        public object? outputs                  { get; set; }
    }
}
