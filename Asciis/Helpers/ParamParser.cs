using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Asciis
{

    public interface IParamParser
    {
        List<string> CommandNames          { get; }
        Regex?       CommandPattern        { get; }
        string?      CommandPatternDisplay { get; }
        string?      CommandDescription    { get; }
        public Type  CommandType           { get; }

        bool   ParseArguments( object obj, string? text = null, List< string >? args = null, bool includeEnvironmentVariables = true);
        string Help();
    }
    
    /// <summary>
    /// Parses the metadata held in <see cref="T"/>
    /// Each implementation will have a usage like:
    ///    [options] ActionName [extra arguments] [options]
    ///
    /// example:
    ///   --verbose --language pirate PLAN battle
    /// what this means is:
    ///   --verbose           will set the `verbose` boolean parameter to true
    ///   --language pirate   will set the language parameter to `pirate`
    ///   PLAN sailing away   PLAN is the action and `sailing away` are the extras
    /// 
    /// example:
    ///   BATTLE --intensity=insane
    /// what this means is:
    ///   BATTLE              BATTLE is the action with no extras
    ///   --intensity=insane  Set the intensity parameter to `insane`
    /// </summary>
    public class ParamParser< T > : IParamParser 
        where T : new()
    {
        public List< string > CommandNames          { get; }
        public Regex?         CommandPattern        { get; }
        public string?        CommandPatternDisplay { get; }
        public string?        CommandDescription    { get; }
        public Type           CommandType           { get; } = typeof(T);

        private readonly List< PropertyMetadata > _properties = new();
        private readonly List< ActionMetadata >   _actions    = new();

        // ReSharper disable once StaticMemberInGenericType
        /// <summary>
        /// [flag][name][separator][value]
        /// flag      = --, -, /
        /// name      = name of parameter
        /// separator = :, = (optional)
        /// value     = value of parameter (optional) 
        /// </summary>
        private static readonly Regex FlagRegex = new( @"^(?<flag>--|-|/)(?<name>[^:=]+)((?<separator>[:=])(?<value>.*))?$" );

        /// <summary>
        /// May throw ArgumentException if <typeparamref name="T"/> if configured incorrectly
        /// </summary>
        public ParamParser()
        {
            var allNames = new List< string >();

            var commandAttribute = CommandType.GetCustomAttribute< CommandAttribute >();
            if (commandAttribute != null)
            {
                CommandNames = commandAttribute.Names;
                if (!string.IsNullOrWhiteSpace( commandAttribute.Pattern ))
                {
                    // Command pattern can be either a valid regex or simple string replacement
                    CommandPatternDisplay = commandAttribute.Pattern;
                    CommandPattern = BuildRegex( commandAttribute.Pattern );
                }
                CommandDescription = commandAttribute.Description;
            }
            else
            {
                CommandNames = new List< string > { CommandType.Name };
                CommandDescription = CommandType.GetCustomAttribute< DescriptionAttribute >()
                                               ?.Description
                                  ?? "";
            }

            var properties = CommandType.GetProperties();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute< ParamAttribute >();
                if (attribute != null)
                {
                    var names = attribute.Pattern.Split( '|' );
                    foreach (var name in names)
                    {
                        if (allNames.Contains( name )) throw new ArgumentException( $"Parameter name [{name}] has already been defined." );
                        allNames.Add( name );
                    }
                    _properties.Add( new PropertyMetadata { Property = property, Attribute = attribute, Names = names.ToList() } );
                }
            }

            var methods = CommandType.GetMethods();
            foreach (var methodInfo in methods)
            {
                var attribute = methodInfo.GetCustomAttribute< ActionAttribute >();
                if (attribute?.Pattern != null)
                {
                    var names = attribute.Pattern.Split( '|' );
                    foreach (var name in names)
                    {
                        if (allNames.Contains( name )) throw new ArgumentException( $"Action name [{name}] has already been defined." );
                        allNames.Add( name );
                    }
                    _actions.Add( new ActionMetadata { Method = methodInfo, Attribute = attribute, Names = names.ToList() } );
                }
            }

            var hasHelp = allNames.Contains( "?" ) || allNames.Contains( "help" );
            if (!hasHelp)
            {
                _actions.Add( new ActionMetadata
                              {
                                  Method = null, 
                                  Attribute = new ActionAttribute( "?|help", "Display help" ), 
                                  Names = new List< string > { "?", "help" }
                              } );
            }
        }

        public static T Parse(string line, bool includeEnvironmentVariables = true)
        {
            var result = new T();
            new ParamParser<T>().ParseArguments(result, line, includeEnvironmentVariables: includeEnvironmentVariables);

            return result;
        }

        public static T Parse( IEnumerable< string > args, bool includeEnvironmentVariables = true )
        {
            var result = new T();
            new ParamParser<T>().ParseArguments(result, args: args.ToList(), includeEnvironmentVariables: includeEnvironmentVariables );

            return result;
        }

        private Regex BuildRegex( string str )
        {
            var properties = CommandType.GetProperties();
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<ParamAttribute>();
                if (attribute != null)
                {
                    var names = attribute.Pattern.Split('|');
                    foreach (var name in names)
                    {
                        // replace any pseudo regex with real regex
                        str = str.Replace( "{" + name + "}", $"(?<{name}>\\w+)" );
                    }
                }
            }

            return new Regex( str );
        }

        public bool ParseArguments( object obj, string? text = null, List< string >? args = null, bool includeEnvironmentVariables = true)
        {
            return ParseArguments( (T)obj, text, args );
        }

        public T ParseArguments( string? text = null, List< string >? args = null, bool includeEnvironmentVariables = true)
        {
            var result = new T();
            ParseArguments( result, text, args, includeEnvironmentVariables );
            return result;
        }

        public bool ParseArguments( T obj, string? text = null, List< string >? args = null, bool includeEnvironmentVariables = true )
        {
            var context = new ParamParseContext();

            // args and text should be mutually exclusive
            if (!string.IsNullOrWhiteSpace( text ))
            {
                var lineArgs = text.ParseArguments();
                if (lineArgs != null)
                {
                    foreach (var arg in lineArgs)
                        Parse( obj, arg, context );
                }
            }

            if (args != null)
            {
                foreach (var arg in args) 
                    Parse( obj, arg, context );
            }

            if (includeEnvironmentVariables)
                ParseEnvironmentVariables(obj);

            if (args == null && CommandPattern != null)
            {
                var groups = CommandPattern.Match( text ?? "" )
                                           .Groups;
                for (int i = 1; i < groups.Count; i++)
                {
                    var match = groups[ i ];

                    context.ParameterName  = match.Name;
                    context.ParameterValue = match.Value;

                    if (!string.IsNullOrWhiteSpace( context.ParameterName ))
                    {
                        if (!CallParam( obj, context.ParameterName, context.ParameterValue )) 
                            throw new ArgumentException( $"Unable to set parameter [{context.ParameterName}] with value [{context.ParameterValue}]" );
                    }
                    context.ClearParameter();
                }
            }

            var called = CallAction( obj, context );

            if (called)
                return true;
            
            if (!called && 
                !context.HelpRequested && 
                HasDefaultAction)
            {
                context.ActionName = null;
                CallAction( obj, context );
            }

            return !context.HelpRequested;
        }

        /// <summary>
        ///     Drop in any environment variables
        /// </summary>
        /// <remarks>Environment variables are case sensitive</remarks>
        public void ParseEnvironmentVariables( T obj )
        {
            var vars = Environment.GetEnvironmentVariables();

            foreach (var property in _properties)
            {
                foreach (var name in property.Names)
                {
                    if (vars.Contains( name ))
                    {
                        var value = vars[ name ]?.ToString() ?? "";
                        CallParam( obj, name, value );
                    }
                }
            }
        }

        private void Parse( T                 obj,
                            string            arg,
                            ParamParseContext context )
        {
            // have we already named the parameter name
            if (context.ParameterName?.HasValue() ?? false)
            {
                if (!CallParam( obj, context.ParameterName, arg )) 
                    throw new ArgumentException( $"Unable to set parameter [{context.ParameterName}] with value [{arg}]" );

                context.ParameterName = null;

                return;
            }

            context.ClearParameter();

            var match = FlagRegex.Match( arg );
            if (!match.Success)
            {
                if (string.IsNullOrWhiteSpace( context.ActionName ) && 
                    context.Extras.Count == 0)
                {
                    if (_actions.FirstOrDefault( a => a.Names.Contains( arg ) ) != null)
                    {
                        context.ActionName = arg;

                        return;
                    }
                }

                context.Extras.Add( arg );

                return;
            }

            //context.ParameterFlag = match.Groups[ "flag" ].Value;
            context.ParameterName = match.Groups[ "name" ].Value;

            var separatorGroup = match.Groups[ "separator" ];
            var valueGroup     = match.Groups[ "value" ];
            if (separatorGroup.Success && valueGroup.Success)
            {
                //context.ParameterSeparator = separatorGroup.Value;
                context.ParameterValue     = valueGroup.Value;

                if (!CallParam( obj, context.ParameterName, context.ParameterValue )) throw new ArgumentException( $"Unable to set parameter [{context.ParameterName}] with value [{context.ParameterValue}]" );
                context.ParameterName = null;
            }
            else if (IsBooleanParameter( context.ParameterName ))
            {
                CallParam( obj, context.ParameterName, "true" );
                context.ClearParameter();
            }
        }

        bool IsBooleanParameter( string parameterName )
        {
            var propertyMetadata = _properties.FirstOrDefault( p => p.Names.Contains( parameterName ) );

            if (propertyMetadata == null) throw new ArgumentException( $"Parameter [{parameterName}] has not been defined" );

            var property = propertyMetadata.Property;
            var tt       = property?.PropertyType;

            // Get the type code so we can switch
            var typeCode = Type.GetTypeCode( tt );

            return typeCode == TypeCode.Boolean;
        }

        public bool CallParam( T obj, string parameterName, string value )
        {
            var propertyMetadata = _properties.FirstOrDefault( p => p.Names.Contains( parameterName ) );
            if (propertyMetadata == null) 
                throw new ArgumentException( $"Parameter [{parameterName}] has not been defined" );

            var property = propertyMetadata.Property;
            if (property == null)
                throw new ArgumentException($"Unknown parameter [{parameterName}]");

            var tt = property.PropertyType;
            if (tt.IsEnum)
            {
                var e = Enum.Parse( tt, value );
                property.SetValue( obj, e );

                return true;
            }

            // Get the type code so we can switch
            var typeCode = Type.GetTypeCode( tt );
            switch (typeCode)
            {
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
            case TypeCode.DateTime:
            case TypeCode.String:
                property.SetValue( obj, Convert.ChangeType( value, typeCode ) );

                return true;
            }

            if (tt.IsGenericType)
            {
                // This currently works for List<T>

                var currentValue = property.GetValue( obj );
                if (currentValue == null)
                {
                    currentValue = Activator.CreateInstance( tt );
                    property.SetValue( obj, currentValue );
                }

                var itemType  = tt.GetGenericArguments()[ 0 ];
                var itemValue = Convert.ChangeType( value, itemType );

                tt.GetMethod( "Add" )
                  ?.Invoke( currentValue, new[] { itemValue } );

                return true;
            }

            throw new ArgumentException( $"Don't know how to set parameter [{parameterName}] with value [{value}]" );
        }

        bool CallAction( T obj, ParamParseContext context )
        {
            var actionMetadata = !string.IsNullOrWhiteSpace( context.ActionName )
                                     ? _actions.FirstOrDefault( p => p.Names.Contains( context.ActionName ) )
                                     : DefaultAction;

            if (actionMetadata == null)
            {
                if (!string.IsNullOrWhiteSpace(context.ActionName))
                    throw new ArgumentException( $"Action [{context.ActionName}] has not been defined" );

                return false; // no action to call
            }

            var method = actionMetadata.Method;
            if (method == null && (context.ActionName == "help" || context.ActionName == "?" || context.ParameterName == "help" || context.ParameterName == "?"))
            {
                context.HelpRequested = true;

                return false;
            }
            if (method == null)
                throw new ArgumentException($"{nameof(actionMetadata)} has no defined method");

            method.Invoke( obj,
                          method.GetParameters().Length == 0
                              ? Array.Empty< object >()
                              : new object?[] { context.Extras } );

            return true;
        }



        public bool HasDefaultAction =>
            _actions.Any( a => a.Attribute?.DefaultAction ?? false );

        ActionMetadata? DefaultAction =>
            _actions.FirstOrDefault( a => a.Attribute?.DefaultAction ?? false );

        public string Help()
        {
            var    sb                         = new StringBuilder();
            string names                      = string.Join( '|', CommandNames );
            if (CommandPattern != null) names += "|" + CommandPattern;

            sb.Append( $"{names}  -  {CommandDescription}\n\n" );

            if (_actions.Count > 0) 
                sb.Append( $"usage: {names} [options] [ActionName] [args]\n" );
            else if (HasDefaultAction) 
                sb.Append( $"usage: {names} [options] [args]\n" );
            else 
                sb.Append( $"usage: {names} [options]\n" );

            int biggest = 0;
            if (_actions.Count > 0)
            {
                foreach (var actionMetadata in _actions)
                {
                    var key = string.Join('|', actionMetadata.Names);
                    biggest = Math.Max( key.Length, biggest );
                }
            }
            if (_properties.Count > 0)
            {
                foreach (var property in _properties)
                {
                    string key = Type.GetTypeCode( property.Property?.PropertyType ) switch
                    {
                        TypeCode.Boolean => string.Join( '|', property.Names ), 
                        _ => string.Join( '|', property.Names ) + "=value"
                    };
                    biggest = Math.Max(key.Length, biggest);
                }
            }

            if (_actions.Count > 0)
            {
                sb.Append( "\nActions\n" );
                
                foreach (var actionMetadata in _actions)
                    sb.Append(
                              $"   {string.Join( '|', actionMetadata.Names ).PadRight( biggest )}    {actionMetadata.Attribute?.Description ?? ""}\n" );
            }

            if (_properties.Count > 0)
            {
                sb.Append( "\nOptions\n" );
                foreach (var property in _properties)
                {
                    switch (Type.GetTypeCode( property.Property?.PropertyType ))
                    {
                    case TypeCode.Boolean:
                        sb.Append( $"   --{string.Join( '|', property.Names ).PadRight( biggest )}  {property.Attribute?.Description??""}\n" );

                        break;
                    default:
                        sb.Append( $"   --{(string.Join( '|', property.Names ) + "=value").PadRight( biggest )}  {property.Attribute?.Description ??""}\n" );

                        break;
                    }
                }
            }

            return sb.ToString();
        }

        class ParamParseContext
        {
            public          string?        ParameterName;
            public          string?        ParameterValue;
            public          string?        ActionName;
            public readonly List< string > Extras = new ();

            public bool HelpRequested;

            public void ClearParameter()
            {
                ParameterName = ParameterValue = null;
            }
        }

        class PropertyMetadata
        {
            public PropertyInfo?   Property;
            public ParamAttribute? Attribute;
            public List< string >  Names = new();
        }

        class ActionMetadata
        {
            public MethodInfo?      Method;
            public ActionAttribute? Attribute;
            public List< string >   Names = new();
        }
    }
}
