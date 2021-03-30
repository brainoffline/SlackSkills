// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;

namespace SlackSkills
{
    public interface IProfileIcons
    {
        string image_24  { get; }
        string image_32  { get; }
        string image_48  { get; }
        string image_72  { get; }
        string image_192 { get; }
        string image_512 { get; }
    }
}
