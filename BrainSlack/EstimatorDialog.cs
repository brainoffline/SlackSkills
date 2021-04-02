using System;
using System.Collections.Generic;

using SlackSkills;
using SlackSkills.Surface;

namespace BrainSlack
{
    public class EstimatorDialog : SlackSurface
    {
        public static List<Option> ScaleOptions = new List<Option>
                                    {
                                        new Option( "Agile 0,½,1,2,3,5,8,13,20,40,:shrug:,:question:",  "Agile" ),
                                        new Option( "Fibonacci 0,1,2,3,5,8,13,21,34,55,:shrug:,:question:", "Fibonacci" ),
                                        new Option( "Linear 0,1,2,3,4,5,6,7,8,9,:shrug:,:question:",        "Linear" ),
                                        new Option( "t-shirt XS,S,M,L,XL,XXL,:shrug:,:question:",           "TShirt" ),
                                        new Option( "Binary 0,1,2,4,8,16,32,64,128,:shrug:,:question:",     "Binary" )
                                    };
        public static List<Option> RiskOptions = new List<Option>
                                   {
                                       new Option( "Ignore",                                     "Ignore" ),
                                       new Option( "t-shirt XS,S,M,L,XL,XXL,:shrug:,:question:", "TShirt" ),
                                   };

        public TextInputElement TitleInput { get; }
        private readonly TextInputElement _descriptionInput;
        public           SelectElement    ScaleSelect { get; }
        public           SelectElement    RiskSelect { get; }

        public string? EstimateTitle =>
            TitleInput?.value;

        public string? EstimateDescription =>
            _descriptionInput?.value;

        public EstimatorDialog(  ISlackApp app, string? title = null, string? description = null ) : base( app )
        {
            TitleInput = new TextInputElement("estimate-title")
                          {
                              placeholder   = "enter the title of the story",
                              initial_value = title,
                              max_length    = 30
                          };
            ScaleSelect = new SelectElement( "estimate-scale-id", "Select scale" )
                          {
                              action_id = "estimate-select-scale", 
                              options = ScaleOptions, 
                              initial_option = ScaleOptions[ 0 ]
                          };
            RiskSelect = new SelectElement("estimate-risk-id", "Select risk")
            {
                action_id      = "estimate-select-scale",
                options        = RiskOptions,
                initial_option = RiskOptions[0]
            };

            _descriptionInput = new TextInputElement( "estimate-description" ) { multiline = true, initial_value = description };
        }

        public override List< Layout > BuildLayouts()
        {
            Title = "Estimate Story";

            ClearLayouts();
            Add( new InputLayout( "Title",       TitleInput,        "estimate-title" ) );
            Add( new InputLayout( "Description", _descriptionInput, "estimate-description" ) { optional = true } );
            Add( new InputLayout( "Scale",       ScaleSelect,       "estimate-scale" ) );
            Add( new InputLayout( "Risk",        RiskSelect,        "estimate-risk" ) );

            return base.BuildLayouts();
        }
    }
}
