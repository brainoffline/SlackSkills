using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

using SlackSkills;
using SlackSkills.Surface;

namespace BrainSlack
{
    public class EstimateSurface : SlackSurface
    {
        public string Description { get; set; }
        public string Scale       { get; set; }
        public string Risk        { get; set; }

        private readonly SectionLayout _messageLayout = new( "estimate-messages" ) { text = "\n" };

        private readonly List< UserVote > _userVotes = new ();

        public bool Revealed { get; set; }

        //new Option( "Agile 0,½,1,2,3,5,8,13,20,40,100,:shrug:,:question:",  "Agile" ),
        //new Option( "Fibonacci 0,1,2,3,5,8,13,21,34,55,:shrug:,:question:", "Fibonacci" ),
        //new Option( "Linear 0,1,2,3,4,5,6,7,8,9,:shrug:,:question:",        "Linear" ),
        //new Option( "t-shirt XS,S,M,L,XL,XXL,:shrug:,:question:",           "TShirt" ),
        //new Option( "Binary 0,1,2,4,8,16,32,64,128,:shrug:,:question:",     "Binary" )

        //new Option( "Ignore",                                     "Ignore" ),
        //new Option( "t-shirt XS,S,M,L,XL,XXL,:shrug:,:question:", "TShirt" ),

        private List<string> Agile     = new() { "0", "½", "1", "2", "3", "5", "8", "13", "20", "40", ":shrug:", ":question:" };
        private List<string> Fibonacci = new() { "0", "1", "2", "3", "5", "8", "13", "21", "34", "55", ":shrug:", ":question:" };
        private List<string> Linear    = new() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ":shrug:", ":question:" };
        private List<string> TShirt    = new() { "XS", "S", "M", "L", "XL", "XXL", ":shrug:", ":question:" };
        private List<string> Binary    = new() { "0", "1", "2", "4", "8", "16", "32", "64", "128", "256", ":shrug:", ":question:" };

        public EstimateSurface( [ NotNull ] ISlackApp app ) : base( app )
        {
        }

        public override List< Layout > BuildLayouts()
        {
            Title ??= "story";

            var fresh    = !HasLayouts;

            if (Revealed)
                ClearLayouts();

            if (fresh || Revealed)
            {
                Add( new HeaderLayout( $"Estimate: {Title}" ) );
                if (!string.IsNullOrEmpty( Description ))
                    Add( new ContextLayout().Add( new MarkdownElement( Description ) ) );
                Add( new DividerLayout() );
            }

            if (fresh && !Revealed)
            {
                List< string > selectedScale;
                switch (Scale)
                {
                case "Fibonacci":
                    selectedScale = Fibonacci;

                    break;
                case "Linear":
                    selectedScale = Linear;

                    break;
                case "TShirt":
                    selectedScale = TShirt;

                    break;
                case "Binary":
                    selectedScale = Binary;

                    break;
                case "Agile":
                    selectedScale = Agile;

                    break;
                default:
                    selectedScale = Agile;

                    break;
                }
                var layout = new ActionsLayout();
                foreach (var item in selectedScale)
                {
                    if (layout.elements.Count >= 5)
                    {
                        Add( layout );
                        layout = new ActionsLayout();
                    }

                    layout.Add( new ButtonElement( item, item ) { Clicked = OnVote, value = item } );
                }
                Add( layout );
            }
            if (fresh || Revealed)
                Add( _messageLayout );

            return base.BuildLayouts();
        }

        private void OnVote( SlackSurface surface, ButtonElement button, BlockActions actions )
        {
            var userVote = _userVotes.FirstOrDefault( uv => uv.User.id == actions.user.id );
            if (userVote == null)
            {
                userVote = new UserVote { User = actions.user };
                _userVotes.Add( userVote );
            }
            userVote.Vote  = button.value;
            userVote.Voted = true;

            _messageLayout.text = string.Join("\n", _userVotes) + "\n";
            SlackApp!.Update( this );
        }

        public void Reveal()
        {
            Revealed = true;
            foreach (var vote in _userVotes)
                vote.Revealed = true;

            _messageLayout.text = string.Join("\n", _userVotes) + "\n";
            SlackApp!.Update(this);
        }


        public class UserVote
        {
            public User User { get; set; }

            public string Name =>
                User.real_name ?? User.name ?? User.id;

            public bool Voted    { get; set; }
            public bool Revealed { get; set; }

            [ CanBeNull ]
            public string Vote { get; set; }
            
            public override string ToString()
            {
                return Revealed ? $"{Name} voted {Vote}" :
                       Voted    ? $"{Name} has Voted"
                                : $"{Name} has joined";
            }
        }
    }
}
