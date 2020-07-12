﻿using System;
using System.Collections.Generic;
using System.Text;
using RegexNodes.Shared;
using RegexNodes.Shared.Nodes;
using RegexNodes.Shared.NodeInputs;

namespace RegexNodes.Tests
{
    class FakeNodeOutput : INodeOutput
    {
        readonly private string output;

        public event EventHandler OutputChanged;

        public Vector2L OutputPos => throw new NotImplementedException();

        public string CssName => throw new NotImplementedException();

        public string CssColor => throw new NotImplementedException();

        public NodeResult CachedOutput => new NodeResult(output, this);

        public FakeNodeOutput(string output) => this.output = output;
    }
}
