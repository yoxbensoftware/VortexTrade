namespace VortexTrade
{
    public static class DevLogHelper
    {
        private static readonly string LogHeader =
            $"# {AppConstants.AppName} — Geliştirme Günlüğü\n\n" +
            "> Bu dosya 10 MB'ı aştığında yeni bir tarihli dosya oluşturulur.\n";

        public static void AppendEntry(DevLogEntry entry, string? logDirectory = null)
        {
            var dir = logDirectory
                ?? Path.GetDirectoryName(typeof(DevLogHelper).Assembly.Location)
                ?? ".";

            var devLogsDir = Path.Combine(dir, AppConstants.DevLogDirectory);
            Directory.CreateDirectory(devLogsDir);

            var targetFile = GetActiveLogFile(devLogsDir);
            var content = FormatEntry(entry);

            if (!File.Exists(targetFile))
            {
                File.WriteAllText(targetFile, LogHeader + content);
            }
            else
            {
                File.AppendAllText(targetFile, content);
            }
        }

        public static string GetActiveLogFile(string devLogsDir)
        {
            var existing = Directory.GetFiles(devLogsDir,
                $"{AppConstants.DevLogFilePrefix}*{AppConstants.DevLogFileExtension}")
                .OrderByDescending(f => f)
                .FirstOrDefault();

            if (existing != null)
            {
                var fileInfo = new FileInfo(existing);
                if (fileInfo.Length < AppConstants.DevLogMaxSizeBytes)
                    return existing;
            }

            return Path.Combine(devLogsDir, GenerateFileName(DateTime.Now));
        }

        public static string GenerateFileName(DateTime date)
        {
            return $"{AppConstants.DevLogFilePrefix}{date.ToString(AppConstants.DevLogDateFormat)}{AppConstants.DevLogFileExtension}";
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
    }
}
