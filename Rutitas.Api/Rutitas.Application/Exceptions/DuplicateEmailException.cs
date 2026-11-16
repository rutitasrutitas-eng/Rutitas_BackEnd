using System;
using System.Collections.Generic;
using System.Text;


namespace Rutitas.Application.Exceptions;

public sealed class DuplicateEmailException : Exception
{
    public DuplicateEmailException()
        : base("Email already registered.")
    {
    }
}
