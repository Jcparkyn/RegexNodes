﻿using Microsoft.AspNetCore.Components;
using RegexNodes.Shared.NodeTypes;
using RegexNodes.Shared.RegexParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace RegexNodes.Shared.Services
{
    public interface INodeHandler
    {
        string CachedOutput { get; }

        event EventHandler OutputChanged;
        event EventHandler OnRequireNoodleRefresh;
        event EventHandler OnRequireNodeGraphRefresh;

        INode SelectedNode { get; }
        NodeTree Tree { get; }

        void DeleteSelectedNode();
        void ForceRefreshNodeGraph();
        void ForceRefreshNoodles();
        void SelectNode(INode node);
        void DeselectAllNodes();
        bool TryCreateTreeFromRegex(string regex);
    }

    public class NodeHandler : INodeHandler
    {
        private NodeTree tree;
        public NodeTree Tree
        {
            get => tree;
            private set
            {
                if(tree != null) tree.OutputChanged -= OnOutputChanged;
                value.OutputChanged += OnOutputChanged;
                tree = value;
            }
        }
        
        public string CachedOutput => Tree.CachedOutput;
        
        public INode SelectedNode { get; private set; }

        public event EventHandler OutputChanged;
        public event EventHandler OnRequireNoodleRefresh;
        public event EventHandler OnRequireNodeGraphRefresh;

        NavigationManager navManager;

        public NodeHandler(NavigationManager navManager)
        {
            this.navManager = navManager;

            var uri = navManager.ToAbsoluteUri(navManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("parse", out var parseString))
            {
                TryCreateTreeFromRegex(parseString);
            }

            if (Tree is null)
            {
                Tree = new NodeTree();
                CreateDefaultNodes(Tree);
            }

            Tree.OutputChanged += OnOutputChanged;
        }

        public bool TryCreateTreeFromRegex(string regex)
        {
            var parseResult = RegexParsers.RegexParser.Parse(regex);

            if (parseResult.Success)
            {
                Tree = parseResult.Value;
                ForceRefreshNodeGraph();
                return true;
            }
            else
            {
                Console.WriteLine("Couldn't parse input: " + parseResult.Error);
                return false;
            }
        }

        private void OnOutputChanged(object sender, EventArgs e)
        {
            OutputChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CreateDefaultNodes(NodeTree tree)
        {
            var defaultOutput = new OutputNode() { Pos = new Vector2L(800, 200) };
            var defaultNode = new TextNode() { Pos = new Vector2L(300, 200) };
            defaultNode.Input.Contents = "fox";
            defaultOutput.PreviousNode = defaultNode;
            tree.AddNode(defaultNode);
            tree.AddNode(defaultOutput);
        }

        public void ForceRefreshNodeGraph()
        {
            OnRequireNodeGraphRefresh?.Invoke(this, EventArgs.Empty);
        }

        public void ForceRefreshNoodles()
        {
            OnRequireNoodleRefresh?.Invoke(this, EventArgs.Empty);
        }

        public void SelectNode(INode node)
        {
            SelectedNode = node;
            ForceRefreshNodeGraph();
        }

        public void DeselectAllNodes()
        {
            if (SelectedNode != null)
            {
                SelectedNode = null;
                ForceRefreshNodeGraph();
            }
        }

        public void DeleteSelectedNode()
        {
            if (SelectedNode != null)
            {
                Tree.DeleteNode(SelectedNode, false); 
            }
        }
    }
}