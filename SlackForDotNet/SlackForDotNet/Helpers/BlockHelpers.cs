using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;


namespace SlackForDotNet
{
    public static class BlockHelpers
    {
        public static string ExtractText(List<Block> blocks)
        {
            var sb = new StringBuilder();

            foreach (var block in blocks)
                ExtractText(sb, block);

            return sb.ToString();
        }

        static void ExtractText([NotNull] StringBuilder sb, [NotNull] Block block)
        {
            if (string.IsNullOrWhiteSpace(block.type))
            {
                var attr = MessageTypes.GetBlockAttributes(block);
                block.type = attr.Type;
            }

            if (block is ElementsBlock elements)
            {
                foreach (var child in elements.elements)
                    ExtractText(sb, child);
            }

            if (block is TextBlock textBlock)
                sb.Append(textBlock.text);
        }

        internal static void PopulateType(this Block? block)
        {
            if (block == null)
                return;
            
            if (string.IsNullOrWhiteSpace(block.type))
            {
                var typeInfo = block.GetType().GetTypeInfo();
                var blockAttr = typeInfo.GetCustomAttribute<SlackBlockAttribute>();
                if (blockAttr != null)
                    block.type = blockAttr.Type;
            }

            switch (block)
            {
            case SectionBlock section:
                section.text?.PopulateType();
                section.fields?.PopulateType();
                section.accessory?.PopulateType();
                break;

            case ImageBlock image:
                image.title?.PopulateType();
                break;

            case InputBlock input:
                input.label?.PopulateType();
                input.hint?.PopulateType();
                break;

            case ButtonElement button:
                button.text?.PopulateType();
                button.confirm?.PopulateType();
                break;

            case DatePickerElement datePicker:
                datePicker.placeholder?.PopulateType();
                datePicker.confirm?.PopulateType();
                break;

            case ImageElement imageElement:
                imageElement.title?.PopulateType();
                break;

            case MultiChannelsSelectElement multiChannelSelect:
                multiChannelSelect.placeholder?.PopulateType();
                multiChannelSelect.confirm?.PopulateType();
                break;

            case MultiConversationsSelectElement multiConversationsSelect:
                multiConversationsSelect.placeholder?.PopulateType();
                multiConversationsSelect.confirm?.PopulateType();
                break;

            case MultiExternalSelectElement multiExternalSelect:
                multiExternalSelect.placeholder?.PopulateType();
                multiExternalSelect.confirm?.PopulateType();
                multiExternalSelect.initial_options?.PopulateType();
                break;

            case MultiStaticSelectElement multiStaticSelect:
                multiStaticSelect.placeholder?.PopulateType();
                multiStaticSelect.confirm?.PopulateType();
                multiStaticSelect.initial_options?.PopulateType();
                multiStaticSelect.options?.PopulateType();
                multiStaticSelect.option_groups?.PopulateType();
                break;

            case MultiUsersSelectElement multiUsersSelect:
                multiUsersSelect.placeholder?.PopulateType();
                multiUsersSelect.confirm?.PopulateType();
                break;

            case OverflowElement overflow:
                overflow.confirm?.PopulateType();
                overflow.options?.PopulateType();
                break;

            case PlainTextInputElement plainTextInput:
                plainTextInput.placeholder?.PopulateType();
                break;

            case RadioButtonsElement radioButtons:
                radioButtons.InitialBlockOption?.PopulateType();
                radioButtons.options?.PopulateType();
                radioButtons.confirm?.PopulateType();
                break;

            case SelectMenuStaticElement selectMenuStatic:
                selectMenuStatic.InitialBlockOption?.PopulateType();
                selectMenuStatic.confirm?.PopulateType();
                selectMenuStatic.option_groups?.PopulateType();
                selectMenuStatic.options?.PopulateType();
                selectMenuStatic.placeholder?.PopulateType();
                break;

            case SelectMenuExternalElement selectMenuExternal:
                selectMenuExternal.InitialBlockOption?.PopulateType();
                selectMenuExternal.confirm?.PopulateType();
                selectMenuExternal.placeholder?.PopulateType();
                break;

            case UsersSelectElement usersSelect:
                usersSelect.confirm?.PopulateType();
                usersSelect.placeholder?.PopulateType();
                break;

            case ConversationsSelectElement conversationsSelect:
                conversationsSelect.confirm?.PopulateType();
                conversationsSelect.placeholder?.PopulateType();
                break;

            case ChannelSelectElement channelSelect:
                channelSelect.confirm?.PopulateType();
                channelSelect.placeholder?.PopulateType();
                break;
            }

            if (block is ElementsBlock elements)
                elements.elements.PopulateType();
        }

        internal static void PopulateType(this IEnumerable<Block>? blocks)
        {
            if (blocks == null) return;
            foreach (var block in blocks)
                block.PopulateType();
        }

        internal static void PopulateType(this BlockOption? block)
        {
            block?.text.PopulateType();
        }

        internal static void PopulateType(this IEnumerable<BlockOption>? blocks)
        {
            if (blocks == null) return;
            foreach (var block in blocks)
                block.text.PopulateType();
        }

        internal static void PopulateType(this IEnumerable<OptionGroup>? blocks)
        {
            if (blocks == null) return;
            foreach (var block in blocks)
            {
                block.label.PopulateType();
                block.options.PopulateType();
            }
        }
    }
}
