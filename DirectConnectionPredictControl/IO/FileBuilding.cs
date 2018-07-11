using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DirectConnectionPredictControl.IO
{
    class FileBuilding
    {
        private string filePath;
        private string fileLength;
        private List<byte[]> fileContent;
        private int len;

        public FileBuilding(string filePath, int len)
        {
            this.filePath = filePath;
            this.len = len;
        }

        /// <summary>
        /// 读取文件到字节数组并切片
        /// </summary>
        /// <returns></returns>
        public List<byte[]> ReadFile()
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            return fileContent;
        }

        public List<byte[]> Split(int len, FileStream fileStream)
        {
            byte[] temp = new byte[len];
            int hasRead = 0;
            //TO DO : 文件切片
            while (hasRead > 0 || (hasRead = fileStream.Read(temp, 0, len)) > 0)
            {

            }
            return null;
        }

        /// <summary>
        /// 将字节数组写入到指定文件中
        /// </summary>
        /// <param name="data">需要存入的字节数组</param>
        public void WriteFile(byte[] data)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write));
            try
            {
                bw.Write(data);
                bw.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
