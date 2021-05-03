using System;
using System.IO;
using System.Security.Cryptography;

namespace SD
{
    class Crypt
    {
        public static void CryptFile(string fileIn, string fileOut, SymmetricAlgorithm algo, byte[] rgbKey, byte[] rgbIV)
        {
            if (string.IsNullOrEmpty(fileIn))
                throw new FileNotFoundException(string.Format("Неверный путь к файлу: {0}.", fileIn));

            if (!File.Exists(fileIn))
                throw new FileNotFoundException(string.Format("Файл '{0}' не найден.", fileIn));

            byte[] buff = null;
            const string CRYPT_EXT = ".crypt";

            using (var sa = algo)
            // Создаем поток для записи зашифрованных данных
            using (var fsw = File.Open(fileOut + CRYPT_EXT, FileMode.Create, FileAccess.Write))

            // Создаем крипто-поток для записи
            using (var cs = new CryptoStream(fsw, sa.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
            {
                // Читаем исходный файл
                using (var fs = File.Open(fileIn, FileMode.Open, FileAccess.Read))
                {
                    // Создаем буфер длинной в файл + 8 байт, для хранения изначальной
                    // длины файла, т.к. при шифровании используется выравнивание по
                    // определенной длине блока (например 512 байт, или 1024)
                    // тем самым файл может немного "раздуть" и оригинал при дешифровке
                    // мы уже не получим
                    buff = new byte[fs.Length + sizeof(long)];
                    // Читаем данные в буфер не с самого начала, а со смещением 8 байт
                    fs.Read(buff, sizeof(long), buff.Length - sizeof(long));
                    /* Записываем в первые 8 байт длину исходного файла
                     * нужно это для того чтобы, после дешифровки не было
                     * лишних данных
                     */
                    int i = 0;
                    foreach (byte @byte in BitConverter.GetBytes(fs.Length))
                        buff[i++] = @byte;
                }
                cs.Write(buff, 0, buff.Length);
                cs.Flush();
            }

            Array.Clear(rgbKey, 0, rgbKey.Length);
            Array.Clear(rgbIV, 0, rgbIV.Length);
        }

        public static void DecryptFile(string fileIn, string fileOut, SymmetricAlgorithm algo, byte[] rgbKey, byte[] rgbIV)
        {
            if (string.IsNullOrEmpty(fileIn))
                throw new FileNotFoundException(string.Format("Неверный путь к файлу: {0}.", fileIn));

            if (!File.Exists(fileIn))
                throw new FileNotFoundException(string.Format("Файл '{0}' не найден.", fileIn));

            byte[] buff = null;
            const string DECRYPT_EXT = ".decrypt";

            using (var sa = algo)
            // Создаем поток для чтения шифрованных данных
            using (var fsr = File.Open(fileIn, FileMode.Open, FileAccess.Read))
            // Создаем крипто-поток для чтения
            using (var cs = new CryptoStream(fsr,
                sa.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Read)
                )
            {
                // Дешифровываем исходный поток данных
                buff = new byte[fsr.Length];
                cs.Read(buff, 0, buff.Length);
                // Пишем дешифрованные данные
                using (var fsw = File.Open(fileOut + DECRYPT_EXT, FileMode.Create, FileAccess.Write))
                {
                    // Читаем записанную длину исходного файла
                    int len = (int)BitConverter.ToInt64(buff, 0);
                    // Пишем только ту часть дешифрованных данных,
                    // которая представляет исходный файл
                    fsw.Write(buff, sizeof(long), len);
                    fsw.Flush();
                }
            }

            Array.Clear(rgbKey, 0, rgbKey.Length);
            Array.Clear(rgbIV, 0, rgbIV.Length);
        }

    }
}
