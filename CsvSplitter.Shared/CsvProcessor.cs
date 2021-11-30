using CsvHelper;
using CsvSplitter.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uno.Extensions.Specialized;
using Windows.Storage;

namespace CsvSplitter.Shared
{
    public class CsvProcessor
    {
        public async Task<int> ProcessCsvAsync(
            StorageFile pickedFile, 
            StorageFolder pickedFolder,
            int size,
            IProgress<float> progress)
        {
            if (size == 0)
            {
                return 0;
            }

            var culture = CultureInfo.GetCultureInfo("en-US");
            List<string[]> records = new List<string[]>();
            string[] headers;

            return await Task.Run<int>(async () =>
            {
                using (var stream = await pickedFile.OpenStreamForReadAsync())
                using (var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8))
                using (var csvReader = new CsvReader(streamReader, culture))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    headers = csvReader.HeaderRecord;
                    int columnCount = headers.Length;

                    while (csvReader.Read())
                    {
                        string[] record = new string[columnCount];
                        for (int i = 0; i < columnCount; i++)
                        {
                            record[i] = csvReader[i];
                        }
                        records.Add(record);
                    }
                }

                if (records.Count <= size)
                {
                    return 0;
                }

                var index = 0;
                var fileName = Path.GetFileNameWithoutExtension(pickedFile.Name);

                var chunks = records.ChunkBy(size);
                var splits = chunks.Count();

                foreach (var chunk in chunks)
                {
                    index++;
                    var splitFileName = fileName + "-" + index + ".csv";

                    StorageFile splitFile = await pickedFolder.CreateFileAsync(splitFileName, CreationCollisionOption.ReplaceExisting);

                    using (var stream = await splitFile.OpenStreamForWriteAsync())
                    using (var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8))
                    using (var csvWriter = new CsvWriter(streamWriter, culture))
                    {
                        var headerString = string.Join(",", headers);
                        streamWriter.WriteLine(headerString);

                        foreach (var fields in chunk)
                        {
                            foreach (var field in fields)
                            {
                                csvWriter.WriteField(field);
                            }

                            csvWriter.NextRecord();
                        }                        
                    }

                    progress.Report((index * 100) / splits);
                }

                return splits;
            });
        }
    }
}
