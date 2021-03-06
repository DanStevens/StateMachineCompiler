﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smc.Syntax
{
    public class SyntaxFormatter : ISyntaxVisitor
    {
        private const int IndentSize = 2;
        private readonly StringBuilder builder = new StringBuilder();

        public string Text => builder.ToString();

        public void Visit(FsmSyntax fsmSyntax)
        {
            fsmSyntax.Headers.Accept(this);
            builder.AppendLine("{");
            fsmSyntax.Logic.Accept(this);
            builder.AppendLine("}");
        }

        public void Visit(Header header)
        {
            builder.Append($"{header.Name}:{header.Value}");
        }

        public void Visit(Transition transition)
        {
            builder.Append(Indent(1));
            transition.State.Accept(this);
            builder.Append(" ");
            transition.Subtransitions.Accept(this);
            builder.AppendLine();
        }

        public void Visit(Subtransition subtransition)
        {
            var ev = FormatOptional(subtransition.Event);
            var nx = FormatOptional(subtransition.NextState);
            var ac = FormatActions(subtransition.Actions);

            builder.Append($"{ev} {nx} {ac}");
        }

        public void Visit(StateSpec state)
        {
            var name = state.IsAbstract ? $"({state.Name})" : state.Name;

            var super = state.Modifiers.Where(x => x.Kind == ModifierKind.SuperState).SelectMany(x => x.Values).Select(x => ":" + x);
            var entry = state.Modifiers.Where(x => x.Kind == ModifierKind.EntryAction).SelectMany(x => x.Values).Select(x => " <" + x);
            var exit = state.Modifiers.Where(x => x.Kind == ModifierKind.ExitAction).SelectMany(x => x.Values).Select(x => " >" + x);

            var modifiers = super.Concat(entry).Concat(exit);

            builder.Append(string.Concat(name, string.Concat(modifiers)));            
        }

        public void Visit(Headers headers)
        {
            foreach (var header in headers)
            {
                header.Accept(this);                
                builder.AppendLine();
            }
        }

        public void Visit(Logic logic)
        {
            builder.AppendLine("{");

            foreach (var transition in logic)
            {               
                transition.Accept(this);
            }

            builder.AppendLine("}");
        }

        public void Visit(Subtransitions subtransitions)
        {
            if (subtransitions.Count == 1)
            {
                subtransitions.First().Accept(this);
            }
            else
            {
                builder.AppendLine();
                builder.AppendLine($"{Indent(1)}{{");

                foreach (var subtransition in subtransitions)
                {
                    builder.Append(Indent(2));
                    subtransition.Accept(this);
                    builder.AppendLine();
                }

                builder.Append($"{Indent(1)}}}");
            }
        }

        private string FormatActions(ICollection<string> actions)
        {
            if (actions.Count == 1) return FormatOptional(actions.First());

            var inner = string.Join(" ", actions);
            return $"{{{inner}}}";
        }

        private static string Indent(int i)
        {
            var r = "";
            for (var j = 0; j < i * IndentSize; j++) r += " ";

            return r;
        }

        private string FormatOptional(string name)
        {
            return name ?? "-";
        }
    }
}