using System;
using System.Data;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using f = System.IO.File;

namespace MintMod.Libraries {
    public class CustomAsync {
        public class File {/*
            public static SafeFileHandle OpenHandle(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read,
                FileShare share = FileShare.Read, FileOptions options = FileOptions.None, long preallocationSize = 0) =>
                new SafeFileHandle(SafeFileHandle.Open(Path.GetFullPath(path), mode, access, share, options, preallocationSize));*/
            public static Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default) {
                // Custom Method from .NET 6
                /*
                if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException();
                if (bytes == null) throw new ArgumentNullException();

                return cancellationToken.IsCancellationRequested
                    ? Task.FromCanceled(cancellationToken)
                    : Core(path, bytes, cancellationToken);

                static async Task Core(string path, byte[] bytes, CancellationToken cancellationToken) {
                    using SafeFileHandle sfh = OpenHandle(path, FileMode.Create, FileAccess.Write, FileShare.Read, FileOptions.Asynchronous);
                    await RandomAccess.WriteAtOffsetAsync(sfh, bytes, 0, cancellationToken).ConfigureAwait(false);
                }
                */
                
                
                var task = Task.Run(() => f.WriteAllBytes(path, bytes), cancellationToken);
                return task;
            }
        }
    }
}