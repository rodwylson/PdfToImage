using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvertPdfToImage
{
    public partial class ConvertPdfToImageForm : Form
    {
        public ConvertPdfToImageForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = txtPathFile.Text;
                string pathSave = folderBrowserDialog1.SelectedPath;

                try
                {
                    PdfDocument document = PdfReader.Open(file);

                    int imageCount = 0;
                    // Iterate pages
                    foreach (PdfPage page in document.Pages)
                    {
                        // Get resources dictionary
                        PdfDictionary resources = page.Elements.GetDictionary("/Resources");


                        if (resources != null)
                        {
                            // Get external objects dictionary
                            PdfDictionary xObjects = resources.Elements.GetDictionary("/XObject");
                            if (xObjects != null)
                            {
                                ICollection<PdfItem> items = xObjects.Elements.Values;
                                // Iterate references to external objects
                                foreach (PdfItem item in items)
                                {
                                    PdfReference reference = item as PdfReference;
                                    if (reference != null)
                                    {
                                        PdfDictionary xObject = reference.Value as PdfDictionary;
                                        // Is external object an image?
                                        if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                                        {
                                            ExportJpegImage(xObject, ref imageCount, pathSave);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    MessageBox.Show(imageCount + " images exported.", "Export Images");


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        static void ExportJpegImage(PdfDictionary image, ref int count, string destinPath)
        {
            // Fortunately JPEG has native support in PDF and exporting an image is just writing the stream to a file.
            byte[] stream = image.Stream.Value;
            FileStream fs = new FileStream(Path.Combine(destinPath, "Image.tiff"), FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(stream);
            bw.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            DialogResult result = openFileDialog1.ShowDialog(); 


            if (result == DialogResult.OK) 
            {
                txtPathFile.Text = openFileDialog1.FileName;

            }
        }
    }
}
