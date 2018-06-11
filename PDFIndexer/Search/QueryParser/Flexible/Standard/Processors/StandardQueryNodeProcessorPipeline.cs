﻿using Lucene.Net.QueryParsers.Flexible.Core.Config;
using Lucene.Net.QueryParsers.Flexible.Core.Processors;

namespace Lucene.Net.QueryParsers.Flexible.Standard.Processors
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /// <summary>
    /// This pipeline has all the processors needed to process a query node tree,
    /// generated by <see cref="Parser.StandardSyntaxParser"/>, already assembled.
    /// <para/>
    /// The order they are assembled affects the results.
    /// <para/>
    /// This processor pipeline was designed to work with
    /// <see cref="Config.StandardQueryConfigHandler"/>.
    /// <para/>
    /// The result query node tree can be used to build a <see cref="Search.Query"/> object using
    /// <see cref="Builders.StandardQueryTreeBuilder"/>.
    /// </summary>
    /// <seealso cref="Builders.StandardQueryTreeBuilder"/>
    /// <seealso cref="Config.StandardQueryConfigHandler"/>
    /// <seealso cref="Parser.StandardSyntaxParser"/>
    public class StandardQueryNodeProcessorPipeline : QueryNodeProcessorPipeline
    {
        public StandardQueryNodeProcessorPipeline(QueryConfigHandler queryConfig)
            : base(queryConfig)
        {
            Add(new WildcardQueryNodeProcessor());
            Add(new MultiFieldQueryNodeProcessor());
            Add(new FuzzyQueryNodeProcessor());
            Add(new MatchAllDocsQueryNodeProcessor());
            Add(new OpenRangeQueryNodeProcessor());
            Add(new NumericQueryNodeProcessor());
            Add(new NumericRangeQueryNodeProcessor());
            Add(new LowercaseExpandedTermsQueryNodeProcessor());
            Add(new TermRangeQueryNodeProcessor());
            Add(new AllowLeadingWildcardProcessor());
            Add(new AnalyzerQueryNodeProcessor());
            Add(new PhraseSlopQueryNodeProcessor());
            //add(new GroupQueryNodeProcessor());
            Add(new BooleanQuery2ModifierNodeProcessor());
            Add(new NoChildOptimizationQueryNodeProcessor());
            Add(new RemoveDeletedQueryNodesProcessor());
            Add(new RemoveEmptyNonLeafQueryNodeProcessor());
            Add(new BooleanSingleChildOptimizationQueryNodeProcessor());
            Add(new DefaultPhraseSlopQueryNodeProcessor());
            Add(new BoostQueryNodeProcessor());
            Add(new MultiTermRewriteMethodProcessor());
        }
    }
}