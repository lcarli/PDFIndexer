﻿using Lucene.Net.Analysis.Util;
using System.Collections.Generic;

namespace Lucene.Net.Analysis.NGram
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
    /// Creates new instances of <see cref="EdgeNGramTokenFilter"/>.
    /// <code>
    /// &lt;fieldType name="text_edgngrm" class="solr.TextField" positionIncrementGap="100"&gt;
    ///   &lt;analyzer&gt;
    ///     &lt;tokenizer class="solr.WhitespaceTokenizerFactory"/&gt;
    ///     &lt;filter class="solr.EdgeNGramFilterFactory" minGramSize="1" maxGramSize="1"/&gt;
    ///   &lt;/analyzer&gt;
    /// &lt;/fieldType&gt;</code>
    /// </summary>
    public class EdgeNGramFilterFactory : TokenFilterFactory
    {
        private readonly int maxGramSize;
        private readonly int minGramSize;
        private readonly string side;

        /// <summary>
        /// Creates a new <see cref="EdgeNGramFilterFactory"/> </summary>
        public EdgeNGramFilterFactory(IDictionary<string, string> args)
            : base(args)
        {
            minGramSize = GetInt32(args, "minGramSize", EdgeNGramTokenFilter.DEFAULT_MIN_GRAM_SIZE);
            maxGramSize = GetInt32(args, "maxGramSize", EdgeNGramTokenFilter.DEFAULT_MAX_GRAM_SIZE);
            side = Get(args, "side", EdgeNGramTokenFilter.Side.FRONT.ToString());
            if (args.Count > 0)
            {
                throw new System.ArgumentException("Unknown parameters: " + args);
            }
        }

        public override TokenStream Create(TokenStream input)
        {
#pragma warning disable 612, 618
            return new EdgeNGramTokenFilter(m_luceneMatchVersion, input, side, minGramSize, maxGramSize);
#pragma warning restore 612, 618
        }
    }
}