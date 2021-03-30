using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SlackSkills.WebApiContracts;

namespace SlackSkills.Context
{
    public class ContextManager
    {
        private readonly ISlackApp                            _slackApp;
        private readonly Dictionary< string, UserContext >    _userContexts    = new();
        private readonly Dictionary< string, ChannelContext > _channelContexts = new();
        private readonly Dictionary< string, BotContext >     _botContexts     = new();

        public ContextManager(ISlackApp slackApp) { _slackApp = slackApp; }

        public async Task<UserContext?> GetUserContext( string? userId )
        {
            if (string.IsNullOrEmpty( userId )) return default;

            if (_userContexts.ContainsKey( userId ))
                return _userContexts[ userId ];

            var userTask    = _slackApp.Send< UserInfo, UserInfoResponse >( new UserInfo { user                   = userId } );
            var profileTask = _slackApp.Send< UserProfileGet, UserProfileGetResponse >( new UserProfileGet { user = userId, include_labels = true } );
            await Task.WhenAll( userTask, profileTask );

            if (userTask.IsFaulted || userTask.Result == null)
                return default;

            var context = new UserContext( userId, userTask.Result.user, profileTask.Result?.profile );
            _userContexts[ userId ] = context;

            return context;
        }

        public async Task< ChannelContext? > GetChannelContext( string? channelId )
        {
            if (string.IsNullOrEmpty( channelId ))
                return default;

            if (_channelContexts.ContainsKey(channelId))
                return _channelContexts[channelId];

            var response = await _slackApp.Send<ConverationInfo, ConversationInfoResponse>(new ConverationInfo { channel = channelId });

            if (response == null || response.ok == false)
                return default;

            var context = new ChannelContext(channelId, response.channel );
            _channelContexts[ channelId ] = context;

            return context;
        }

        public async Task<BotContext?> GetBotContext(string? botId, string? teamId = null)
        {
            if (string.IsNullOrEmpty(botId))
                return default;

            if (_botContexts.ContainsKey(botId))
                return _botContexts[botId];

            var response = await _slackApp.Send<BotInfoRequest, BotInfoResponse>(new BotInfoRequest { bot = botId, team_id = teamId });

            if (response == null || response.ok == false)
                return default;

            var context = new BotContext(botId, response.bot);
            _botContexts[botId] = context;

            return context;
        }

    }
}
