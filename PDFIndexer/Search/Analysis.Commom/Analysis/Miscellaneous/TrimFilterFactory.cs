﻿using Lucene.Net.Analysis.Util;
using System.Collections.Generic;

namespace Lucene.Net.Analysis.Miscellaneous
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
    /// Factory for <see cref="TrimFilter"/>.
    /// <code>
    /// &lt;fieldType name="text_trm" class="solr.TextField" positionIncrementGap="100"&gt;
    ///   &lt;analyzer&gt;
    ///     &lt;tokenizer class="solr.NGramTokenizerFactory"/&gt;
    ///     &lt;filter class="solr.TrimFilterFactory" /&gt;
    ///   &lt;/analyzer&gt;
    /// &lt;/fieldType&gt;</code>
    /// </summary>
    /// <seealso cref="TrimFilter"/>
    public class TrimFilterFactory : TokenFilterFactory
    {
        protected readonly bool m_updateOffsets;

        /// <summary>
        /// Creates a new <see cref="TrimFilterFactory"/> </summary>
        public TrimFilterFactory(IDictionary<string, string> args)
            : base(args)
        {
            m_updateOffsets = GetBoolean(args, "updateOffsets", false);
            if (args.Count > 0)
            {
                throw new System.ArgumentException("Unknown parameters: " + args);
            }
        }

        public override TokenStream Create(TokenStream input)
        {
#pragma warning disable 612, 618
            var filter = new TrimFilter(m_luceneMatchVersion, input, m_updateOffsets);
#pragma warning restore 612, 618
            return filter;
        }
    }
}