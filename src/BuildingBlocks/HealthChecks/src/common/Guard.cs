// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

static class Guard
{
    public static void ArgumentNotNull(string argumentName, object value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void ArgumentNotNullOrEmpty(string argumentName, string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be an empty string.", argumentName);
        }
    }

    // Use IReadOnlyCollection<T> instead of IEnumerable<T> to discourage double enumeration
    public static void ArgumentNotNullOrEmpty<T>(string argumentName, IReadOnlyCollection<T> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(argumentName);
        }
        if (items.Count == 0)
        {
            throw new ArgumentException("Collection must contain at least one item.", argumentName);
        }
    }

    public static void ArgumentValid(bool valid, string argumentName, string exceptionMessage)
    {
        if (!valid)
        {
            throw new ArgumentException(exceptionMessage, argumentName);
        }
    }

    public static void OperationValid(bool valid, string exceptionMessage)
    {
        if (!valid)
        {
            throw new InvalidOperationException(exceptionMessage);
        }
    }
}
