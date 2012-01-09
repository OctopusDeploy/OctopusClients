using System;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace
public static class TaskExtensions
// ReSharper restore CheckNamespace
{
    public static TResult Execute<TResult>(this Task<TResult> task)
    {
        if (task.Status == TaskStatus.Created)
        {
            task.RunSynchronously();
        }

        return task.Result;
    }
}