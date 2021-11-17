using System.Threading.Tasks;

namespace MintyLoader
{
    static class Extensions
    {
        public static void NoAwait(this Task task, string taskDescription = null)
        {
            task.ContinueWith(tsk =>
            {
                if (tsk.IsFaulted)
                    MelonLoader.MelonLogger.Error($"Free-floating Task {(taskDescription == null ? "" : $"({taskDescription})")}}} failed with exception: {tsk.Exception}");
            });
        }
    }
}
