namespace Memstate
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class FileJournalReader : IJournalReader
    {
    private readonly FileStream _journalStream;
    private readonly ISerializer _serializer;

    public FileJournalReader(string fileName, ISerializer serializer)
    {
        _journalStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        _serializer = serializer;
    }

    public Task DisposeAsync()
    {
        return Task.Run((Action)_journalStream.Dispose);
    }

    public IEnumerable<JournalRecord> GetRecords(long fromRecord = 0)
    {
        foreach (var records in _serializer.ReadObjects<JournalRecord[]>(_journalStream))
        {
            foreach (var record in records)
            {
                if (record.RecordNumber >= fromRecord)
                {
                    yield return record;
                }
            }
        }
    }
    }
}