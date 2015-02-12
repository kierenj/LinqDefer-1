//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
namespace LinqDefer.ExpressionAnalysers
{
    /// <summary>
    /// Classification decorator class, applied to the main Select() method call expression of a LINQ query
    /// </summary>
    internal class MainSelectCallClassification : ExpressionClassification
    {
    }
}