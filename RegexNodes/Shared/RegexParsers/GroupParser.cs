﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using RegexNodes.Shared.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using static RegexNodes.Shared.RegexParsers.ParsersShared;

namespace RegexNodes.Shared.RegexParsers
{
    public static class GroupParser
    {
        /// <summary>
        /// Parse any type of group. This includes anything in (non-escaped) parentheses.
        /// </summary>
        public static Parser<char, Node> ParseGroup =>
            Char('?').Then(OneOf(
                LookaroundParser.ParseLookaround.Cast<Node>(),
                OtherGroup.Cast<Node>(),
                IfElseParser.ParseIfElse.Cast<Node>()
                ))
            .Or(CaptureGroup.Cast<Node>())
            .Between(Char('('), Char(')'));

        private static Parser<char, GroupNode> OtherGroup =>
            Map((node, contents) => node.WithContents(contents),
                NormalGroupPrefix,
                Rec(() => RegexParser.ParseRegex));

        private static Parser<char, GroupNode> CaptureGroup =>
            Rec(() => RegexParser.ParseRegex)
            .Select(contents =>
                CreateWithType(GroupNode.GroupTypes.capturing)
                .WithContents(contents));

        private static Parser<char, GroupNode> NormalGroupPrefix =>
            OneOf(
                Char(':').Select(c => CreateWithType(GroupNode.GroupTypes.nonCapturing)),
                Char('>').Select(c => CreateWithType(GroupNode.GroupTypes.atomic)),
                GroupName.Select(name => CreateWithName(name))
                );

        private static Parser<char, string> GroupName =>
            OneOf("<'")
            .Then(AnyCharExcept(">'").ManyString())
            .Before(OneOf(">'"));

        private static GroupNode CreateWithType(string groupType)
        {
            var node = new GroupNode();
            node.InputGroupType.DropdownValue = groupType;
            return node;
        }

        private static GroupNode CreateWithName(string name)
        {
            var node = new GroupNode();
            node.InputGroupType.DropdownValue = GroupNode.GroupTypes.named;
            node.GroupName.Contents = name;
            return node;
        }

        private static GroupNode WithContents(this GroupNode node, Node contents)
        {
            node.Input.ConnectedNode = contents;
            return node;
        }
    }
}