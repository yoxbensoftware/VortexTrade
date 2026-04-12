namespace VortexTrade
{
    public static class DevLogHelper
    {
        private static readonly string LogDirectory =
            Path.GetDirectoryName(typeof(DevLogHelper).Assembly.Location) ?? ".";

        public static void AppendEntry(DevLogEntry entry, string? logDirectory = null)
        {
            var dir = logDirectory ?? LogDirectory;
            var logPath = Path.Combine(dir, AppConstants.DevLogFileName);

            RotateIfNeeded(logPath, dir);

            var content = FormatEntry(entry);

            if (!File.Exists(logPath))
            {
                File.WriteAllText(logPath, $"# {AppConstants.AppName} — Geliştirme Günlüğü\n\n{content}");
            }
            else
            {
                File.AppendAllText(logPath, content);
            }
        }

        private static string FormatEntry(DevLogEntry entry)
        {
            return $"""

                ---

                ### {entry.Version} — {entry.Date:yyyy-MM-dd HH:mm}

                | Alan | Değer |
                |---|---|
                | **Tarih** | {entry.Date:yyyy-MM-dd HH:mm} |
                | **Geliştiren** | {entry.Developer} |
                | **Bilgisayar** | {entry.MachineName} |

                **Yapılan Geliştirme:**
                {entry.Description}

                """;
        }

        private static void RotateIfNeeded(string logPath, string directory)
        {
            if (!File.Exists(logPath))
                return;

            var fileInfo = new FileInfo(logPath);
            if (fileInfo.Length < AppConstants.DevLogMaxSizeBytes)
                return;

            var archiveIndex = 1;
            string archivePath;
            do
            {
                archivePath = Path.Combine(directory,
                    $"DEVLOG_archive_{archiveIndex:D3}.md");
                archiveIndex++;
            } while (File.Exists(archivePath));

            File.Move(logPath, archivePath);
        }
    }
}
