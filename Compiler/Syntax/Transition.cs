﻿using System.Linq;

namespace Smc.Syntax
{
    public class Transition : ISyntax
    {
        public StateSpec State { get; }
        public Subtransitions Subtransitions { get; }

        public Transition(StateSpec state, Subtransitions subtransitions)
        {
            State = state;
            Subtransitions = subtransitions;
        }

        public override string ToString()
        {
            var subtransitionsStr = string.Join(" ", Subtransitions.Select(x => x.ToString()));
            return $@"{State}: {{{subtransitionsStr}}}";
        }

        public void Accept(ISyntaxVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}