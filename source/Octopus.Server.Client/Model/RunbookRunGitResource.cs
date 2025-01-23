using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model;

public class RunbookRunGitResource(RunbookRunResource[] resources)
{
    public RunbookRunResource[] Resources { get; set; } = resources;
}