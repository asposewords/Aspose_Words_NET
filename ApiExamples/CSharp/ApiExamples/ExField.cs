//////////////////////////////////////////////////////////////////////////
// Copyright 2001-2018 Aspose Pty Ltd. All Rights Reserved.
//
// This file is part of Aspose.Words. The source code in this file
// is only intended as a supplement to the documentation, and is provided
// "as is", without warranty of any kind, either expressed or implied.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Aspose.Words;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Fields;
using Aspose.Words.MailMerging;
using Aspose.Words.Replacing;
using NUnit.Framework;
#if !(NETSTANDARD2_0 || __MOBILE__)
using Aspose.BarCode.BarCodeRecognition;
#endif

namespace ApiExamples
{
    [TestFixture]
    public class ExField : ApiExampleBase
    {
        [Test]
        public void UpdateToc()
        {
            Document doc = new Document();

            //ExStart
            //ExId:UpdateTOC
            //ExSummary:Shows how to completely rebuild TOC fields in the document by invoking field update.
            doc.UpdateFields();
            //ExEnd
        }

        [Test]
        public void GetFieldFromDocument()
        {
            //ExStart
            //ExFor:FieldType
            //ExFor:FieldChar
            //ExFor:FieldChar.FieldType
            //ExFor:FieldChar.IsDirty
            //ExFor:FieldChar.IsLocked
            //ExFor:FieldChar.GetField
            //ExFor:Field.IsLocked
            //ExId:GetField
            //ExSummary:Demonstrates how to retrieve the field class from an existing FieldStart node in the document.
            Document doc = new Document(MyDir + "Document.TableOfContents.doc");

            FieldChar fieldStart = (FieldChar)doc.GetChild(NodeType.FieldStart, 0, true);
            Assert.AreEqual(FieldType.FieldTOC, fieldStart.FieldType);
            Assert.AreEqual(true, fieldStart.IsDirty);
            Assert.AreEqual(false, fieldStart.IsLocked);

            // Retrieve the facade object which represents the field in the document.
            Field field = fieldStart.GetField();

            Assert.AreEqual(false, field.IsLocked);
            Assert.AreEqual(" TOC \\o \"1-3\" \\h \\z \\u ", field.GetFieldCode());

            // This updates only this field in the document.
            field.Update();         
            //ExEnd
        }

        [Test]
        public void CreateRevNumFieldWithFieldBuilder()
        {
            //ExStart
            //ExFor:FieldBuilder.#ctor(FieldType)
            //ExFor:FieldBuilder.BuildAndInsert(Inline)
            //ExFor:FieldRevNum
            //ExSummary:Builds and inserts a field into the document before the specified inline node
            Document doc = new Document();
            Run run = DocumentHelper.InsertNewRun(doc, " Hello World!", 0);

            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldRevisionNum);
            fieldBuilder.BuildAndInsert(run);

            doc.UpdateFields();
            //ExEnd
            MemoryStream dstStream = new MemoryStream();
            doc.Save(dstStream, SaveFormat.Docx);

            FieldRevNum revNum = (FieldRevNum)doc.Range.Fields[0];
            Assert.NotNull(revNum);
        }

        [Test]
        public void CreateRevNumFieldByDocumentBuilder()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertField("REVNUM MERGEFORMAT");

            MemoryStream dstStream = new MemoryStream();
            doc.Save(dstStream, SaveFormat.Docx);

            FieldRevNum revNum = (FieldRevNum)doc.Range.Fields[0];
            Assert.NotNull(revNum);
        }

        [Test]
        public void CreateInfoFieldWithFieldBuilder()
        {
            Document doc = new Document();
            Run run = DocumentHelper.InsertNewRun(doc, " Hello World!", 0);

            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldInfo);
            fieldBuilder.BuildAndInsert(run);

            doc.UpdateFields();

            MemoryStream dstStream = new MemoryStream();
            doc.Save(dstStream, SaveFormat.Docx);

            FieldInfo info = (FieldInfo)doc.Range.Fields[0];
            Assert.NotNull(info);
        }

        [Test]
        public void CreateInfoFieldWithDocumentBuilder()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertField("INFO MERGEFORMAT");

            MemoryStream dstStream = new MemoryStream();
            doc.Save(dstStream, SaveFormat.Docx);

            FieldInfo info = (FieldInfo)doc.Range.Fields[0];
            Assert.NotNull(info);
        }

        [Test]
        public void GetFieldFromFieldCollection()
        {
            //ExStart
            //ExId:GetFieldFromFieldCollection
            //ExSummary:Demonstrates how to retrieve a field using the range of a node.
            Document doc = new Document(MyDir + "Document.TableOfContents.doc");

            Field field = doc.Range.Fields[0];

            // This should be the first field in the document - a TOC field.
            Console.WriteLine(field.Type);
            //ExEnd
        }

        [Test]
        public void InsertFieldNone()
        {
            //ExStart
            //ExFor:FieldUnknown
            //ExSummary:Shows how to work with 'FieldNone' field in a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertField(FieldType.FieldNone, false);

            MemoryStream stream = new MemoryStream();
            doc.Save(stream, SaveFormat.Docx);

            FieldUnknown fieldUnknown = (FieldUnknown)doc.Range.Fields.FirstOrDefault(p => p.Type == FieldType.FieldNone);
            if (fieldUnknown != null)
                Assert.AreEqual(FieldType.FieldNone, fieldUnknown.Type);
            else
                Assert.Fail("FieldUnknown doesn't exist");
            //ExEnd
        }


        [Test]
        public void InsertTcField()
        {
            //ExStart
            //ExId:InsertTCField
            //ExSummary:Shows how to insert a TC field into the document using DocumentBuilder.
            // Create a blank document.
            Document doc = new Document();

            // Create a document builder to insert content with.
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a TC field at the current document builder position.
            builder.InsertField("TC \"Entry Text\" \\f t");
            //ExEnd
        }

        [Test]
        public void ChangeLocale()
        {
            // Create a blank document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertField("MERGEFIELD Date");

            //ExStart
            //ExId:ChangeCurrentCulture
            //ExSummary:Shows how to change the culture used in formatting fields during update.
            // Store the current culture so it can be set back once mail merge is complete.
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            // Set to German language so dates and numbers are formatted using this culture during mail merge.
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");

            // Execute mail merge.
            doc.MailMerge.Execute(new[] { "Date" }, new object[] { DateTime.Now });

            // Restore the original culture.
            Thread.CurrentThread.CurrentCulture = currentCulture;
            //ExEnd

            doc.Save(ArtifactsDir + "Field.ChangeLocale.doc");
        }

        [Test]
        public void RemoveTocFromDocument()
        {
            //ExStart
            //ExFor:CompositeNode.GetChildNodes(NodeType, Boolean)
            //ExId:RemoveTableOfContents
            //ExSummary:Demonstrates how to remove a specified TOC from a document.
            // Open a document which contains a TOC.
            Document doc = new Document(MyDir + "Document.TableOfContents.doc");

            // Remove the first TOC from the document.
            Field tocField = doc.Range.Fields[0];
            tocField.Remove();

            // Save the output.
            doc.Save(ArtifactsDir + "Document.TableOfContentsRemoveTOC.doc");
            //ExEnd
        }

        [Test]
        //ExStart
        //ExId:TCFieldsRangeReplace
        //ExSummary:Shows how to find and insert a TC field at text in a document.
        public void InsertTcFieldsAtText()
        {
            Document doc = new Document();

            FindReplaceOptions options = new FindReplaceOptions();
            options.ReplacingCallback = new InsertTcFieldHandler("Chapter 1", "\\l 1");

            // Insert a TC field which displays "Chapter 1" just before the text "The Beginning" in the document.
            doc.Range.Replace(new Regex("The Beginning"), "", options);
        }

        private class InsertTcFieldHandler : IReplacingCallback
        {
            // Store the text and switches to be used for the TC fields.
            private readonly string mFieldText;
            private readonly string mFieldSwitches;

            /// <summary>
            /// The display text and switches to use for each TC field. Display name can be an empty String or null.
            /// </summary>
            public InsertTcFieldHandler(string text, string switches)
            {
                mFieldText = text;
                mFieldSwitches = switches;
            }

            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs args)
            {
                // Create a builder to insert the field.
                DocumentBuilder builder = new DocumentBuilder((Document)args.MatchNode.Document);
                // Move to the first node of the match.
                builder.MoveTo(args.MatchNode);

                // If the user specified text to be used in the field as display text then use that, otherwise use the 
                // match String as the display text.
                String insertText;

                if (!string.IsNullOrEmpty(mFieldText))
                    insertText = mFieldText;
                else
                    insertText = args.Match.Value;

                // Insert the TC field before this node using the specified String as the display text and user defined switches.
                builder.InsertField(string.Format("TC \"{0}\" {1}", insertText, mFieldSwitches));

                // We have done what we want so skip replacement.
                return ReplaceAction.Skip;
            }
        }

        //ExEnd

        [Test]
        [Description("WORDSNET-16037")]
        public void InsertAndUpdateDirtyField()
        {
            //ExStart
            //ExFor:Field.IsDirty
            //ExFor:LoadOptions.UpdateDirtyFields
            //ExSummary:Shows how to use special property for updating field result
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Field fieldToc = builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");
            fieldToc.IsDirty = true;
            
            MemoryStream stream = new MemoryStream();
            doc.Save(stream, SaveFormat.Docx);
            
            Assert.IsTrue(doc.Range.Fields[0].IsDirty);

            LoadOptions loadOptions = new LoadOptions { UpdateDirtyFields = false };
            doc = new Document(stream, loadOptions);
            //ExEnd
        }

        [Test]
        public void InsertFieldWithFieldBuilderException()
        {
            Document doc = new Document();

            //Add some text into the paragraph
            Run run = DocumentHelper.InsertNewRun(doc, " Hello World!", 0);

            FieldArgumentBuilder argumentBuilder = new FieldArgumentBuilder();
            argumentBuilder.AddField(new FieldBuilder(FieldType.FieldMergeField));
            argumentBuilder.AddNode(run);
            argumentBuilder.AddText("Text argument builder");

            FieldBuilder fieldBuilder = new FieldBuilder(FieldType.FieldIncludeText);

            Assert.That(
                () => fieldBuilder.AddArgument(argumentBuilder).AddArgument("=").AddArgument("BestField")
                    .AddArgument(10).AddArgument(20.0).BuildAndInsert(run), Throws.TypeOf<ArgumentException>());
        }

#if !(NETSTANDARD2_0 || __MOBILE__)
        [Test]
        public void BarCodeWord2Pdf()
        {
            Document doc = new Document(MyDir + "Field.BarCode.docx");

            // Set custom barcode generator
            doc.FieldOptions.BarcodeGenerator = new CustomBarcodeGenerator();

            doc.Save(ArtifactsDir + "Field.BarCode.pdf");

            BarCodeReader barCode = BarCodeReaderPdf(ArtifactsDir + "Field.BarCode.pdf");
            Assert.AreEqual("QR", barCode.GetCodeType().ToString());
        }

        private BarCodeReader BarCodeReaderPdf(String filename)
        {
            //Set license for Aspose.BarCode
            Aspose.BarCode.License licenceBarCode = new Aspose.BarCode.License();
            licenceBarCode.SetLicense(LicenseDir + "Aspose.Total.lic");

            //bind the pdf document
            Aspose.Pdf.Facades.PdfExtractor pdfExtractor = new Aspose.Pdf.Facades.PdfExtractor();
            pdfExtractor.BindPdf(filename);

            //set page range for image extraction
            pdfExtractor.StartPage = 1;
            pdfExtractor.EndPage = 1;

            pdfExtractor.ExtractImage();

            //save image to stream
            MemoryStream imageStream = new MemoryStream();
            pdfExtractor.GetNextImage(imageStream);
            imageStream.Position = 0;

            //recognize the barcode from the image stream above
            BarCodeReader barcodeReader = new BarCodeReader(imageStream, DecodeType.QR);
            while (barcodeReader.Read())
            {
                Console.WriteLine("Codetext found: " + barcodeReader.GetCodeText() + ", Symbology: " +
                                  barcodeReader.GetCodeType());
            }

            //close the reader
            barcodeReader.Close();

            return barcodeReader;
        }
#endif
        //For assert result of the test you need to open document and check that image are added correct and without truncated inside frame
        [Test]
        public void UpdateFieldIgnoringMergeFormat()
        {
            //ExStart
            //ExFor:Field.Update(bool)
            //ExSummary:Shows a way to update a field ignoring the MERGEFORMAT switch
            LoadOptions loadOptions = new LoadOptions { PreserveIncludePictureField = true };

            Document doc = new Document(MyDir + "Field.UpdateFieldIgnoringMergeFormat.docx", loadOptions);

            foreach (Field field in doc.Range.Fields)
            {
                if (field.Type.Equals(FieldType.FieldIncludePicture))
                {
                    FieldIncludePicture includePicture = (FieldIncludePicture)field;

                    includePicture.SourceFullName = MyDir + "Images/dotnet-logo.png";
                    includePicture.Update(true);
                }
            }

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.UpdateFieldIgnoringMergeFormat.docx");
            //ExEnd
        }

        [Test]
        public void FieldFormat()
        {
            //ExStart
            //ExFor:Field.Format
            //ExFor:FieldFormat
            //ExFor:FieldFormat.DateTimeFormat
            //ExFor:FieldFormat.NumericFormat
            //ExFor:FieldFormat.GeneralFormats
            //ExFor:GeneralFormat
            //ExFor:GeneralFormatCollection
            //ExFor:GeneralFormatCollection.Add(GeneralFormat)
            //ExFor:GeneralFormatCollection.Count
            //ExFor:GeneralFormatCollection.Item(Int32)
            //ExFor:GeneralFormatCollection.Remove(GeneralFormat)
            //ExFor:GeneralFormatCollection.RemoveAt(Int32)
            //ExSummary:Shows how to format fields
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Use a document builder to insert field with no format
            Field field = builder.InsertField("= 2 + 3");

            // We can format our field here instead of in the field code
            FieldFormat format = field.Format;
            format.NumericFormat = "$###.00";
            field.Update();

            // Apply a date/time format
            field = builder.InsertField("DATE");
            format = field.Format;
            format.DateTimeFormat = "dddd, MMMM dd, yyyy";
            field.Update();

            // Apply 2 general formats at the same time
            field = builder.InsertField("= 25 + 33");
            format = field.Format;
            format.GeneralFormats.Add(GeneralFormat.LowercaseRoman);
            format.GeneralFormats.Add(GeneralFormat.Upper);
            field.Update();

            Assert.AreEqual("LVIII", field.Result);
            Assert.AreEqual(2, format.GeneralFormats.Count);
            Assert.AreEqual(GeneralFormat.LowercaseRoman, format.GeneralFormats[0]);

            // Removing field formats
            format.GeneralFormats.Remove(GeneralFormat.LowercaseRoman);
            format.GeneralFormats.RemoveAt(0);
            Assert.AreEqual(0, format.GeneralFormats.Count);
            field.Update();

            // Our field has no general formats left and is back to default form
            Assert.AreEqual("58", field.Result);
            //ExEnd
        }

        [Test]
        public void UnlinkAllFieldsInDocument()
        {
            //ExStart
            //ExFor:Document.UnlinkFields
            //ExSummary:Shows how to unlink all fields in the document
            Document doc = new Document(MyDir + "Field.UnlinkFields.docx");

            doc.UnlinkFields();
            //ExEnd

            String paraWithFields = DocumentHelper.GetParagraphText(doc, 0);
            Assert.AreEqual("Fields.Docx   Элементы указателя не найдены.     1.\r", paraWithFields);
        }

        [Test]
        public void UnlinkAllFieldsInRange()
        {
            //ExStart
            //ExFor:Range.UnlinkFields
            //ExSummary:Shows how to unlink all fields in range
            Document doc = new Document(MyDir + "Field.UnlinkFields.docx");

            Section newSection = (Section)doc.Sections[0].Clone(true);
            doc.Sections.Add(newSection);

            doc.Sections[1].Range.UnlinkFields();
            //ExEnd

            String secWithFields = DocumentHelper.GetSectionText(doc, 1);
            Assert.AreEqual(
                "Fields.Docx   Элементы указателя не найдены.     3.\rОшибка! Не указана последовательность.    Fields.Docx   Элементы указателя не найдены.     4.\r\r\r\r\r\f",
                secWithFields);
        }

        [Test]
        public void UnlinkSingleField()
        {
            //ExStart
            //ExFor:Field.Unlink
            //ExSummary:Shows how to unlink specific field
            Document doc = new Document(MyDir + "Field.UnlinkFields.docx");
            doc.Range.Fields[1].Unlink();
            //ExEnd

            String paraWithFields = DocumentHelper.GetParagraphText(doc, 0);
            Assert.AreEqual(
                "\u0013 FILENAME  \\* Caps  \\* MERGEFORMAT \u0014Fields.Docx\u0015   Элементы указателя не найдены.     \u0013 LISTNUM  LegalDefault \u0015\r",
                paraWithFields);
        }

        [Test]
        public void UpdatePageNumbersInToc()
        {
            Document doc = new Document(MyDir + "Field.UpdateTocPages.docx");

            Node startNode = DocumentHelper.GetParagraph(doc, 2);
            Node endNode = null;

            NodeCollection paragraphCollection = doc.GetChildNodes(NodeType.Paragraph, true);

            foreach (Paragraph para in paragraphCollection.OfType<Paragraph>())
            {
                // Check all runs in the paragraph for the first page breaks.
                foreach (Run run in para.Runs.OfType<Run>())
                {
                    if (run.Text.Contains(ControlChar.PageBreak))
                    {
                        endNode = run;
                        break;
                    }
                }
            }

            if (startNode != null && endNode != null)
            {
                RemoveSequence(startNode, endNode);

                startNode.Remove();
                endNode.Remove();
            }

            NodeCollection fStart = doc.GetChildNodes(NodeType.FieldStart, true);

            foreach (FieldStart field in fStart.OfType<FieldStart>())
            {
                FieldType fType = field.FieldType;
                if (fType == FieldType.FieldTOC)
                {
                    Paragraph para = (Paragraph)field.GetAncestor(NodeType.Paragraph);
                    para.Range.UpdateFields();
                    break;
                }
            }

            doc.Save(ArtifactsDir + "Field.UpdateTocPages.docx");
        }

        private void RemoveSequence(Node start, Node end)
        {
            Node curNode = start.NextPreOrder(start.Document);
            while (curNode != null && !curNode.Equals(end))
            {
                //Move to next node
                Node nextNode = curNode.NextPreOrder(start.Document);

                //Check whether current contains end node
                if (curNode.IsComposite)
                {
                    CompositeNode curComposite = (CompositeNode)curNode;
                    if (!curComposite.GetChildNodes(NodeType.Any, true).Contains(end) &&
                        !curComposite.GetChildNodes(NodeType.Any, true).Contains(start))
                    {
                        nextNode = curNode.NextSibling;
                        curNode.Remove();
                    }
                }
                else
                {
                    curNode.Remove();
                }

                curNode = nextNode;
            }
        }

        [Test]
        public void DropDownItemCollection()
        {
            //ExStart
            //ExFor:Fields.DropDownItemCollection
            //ExFor:Fields.DropDownItemCollection.Add(String)
            //ExFor:Fields.DropDownItemCollection.Clear
            //ExFor:Fields.DropDownItemCollection.Contains(String)
            //ExFor:Fields.DropDownItemCollection.Count
            //ExFor:Fields.DropDownItemCollection.GetEnumerator
            //ExFor:Fields.DropDownItemCollection.IndexOf(String)
            //ExFor:Fields.DropDownItemCollection.Insert(Int32, String)
            //ExFor:Fields.DropDownItemCollection.Item(Int32)
            //ExFor:Fields.DropDownItemCollection.Remove(String)
            //ExFor:Fields.DropDownItemCollection.RemoveAt(Int32)
            //ExSummary:Shows how to insert a combo box field and manipulate the elements in its item collection.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Use a document builder to create and populate a combo box
            string[] items = { "One", "Two", "Three" };
            FormField comboBoxField = builder.InsertComboBox("DropDown", items, 0);

            // Get the list of drop down items
            DropDownItemCollection dropDownItems = comboBoxField.DropDownItems;

            Assert.AreEqual(3, dropDownItems.Count);
            Assert.AreEqual("One", dropDownItems[0]);
            Assert.AreEqual(1, dropDownItems.IndexOf("Two"));
            Assert.IsTrue(dropDownItems.Contains("Three"));

            // We can add an item to the end of the collection or insert it at a desired index
            dropDownItems.Add("Four");
            dropDownItems.Insert(3, "Three and a half");
            Assert.AreEqual(5, dropDownItems.Count);

            // Iterate over the collection and print every element
            using (IEnumerator<string> dropDownCollectionEnumerator = dropDownItems.GetEnumerator())
            {
                while (dropDownCollectionEnumerator.MoveNext())
                {
                    string currentItem = dropDownCollectionEnumerator.Current;
                    Console.WriteLine(currentItem);
                }
            }

            // We can remove elements in the same way we added them
            dropDownItems.Remove("Four");
            dropDownItems.RemoveAt(3);
            Assert.IsFalse(dropDownItems.Contains("Three and a half"));
            Assert.IsFalse(dropDownItems.Contains("Four"));

            doc.Save(ArtifactsDir + "Fields.DropDownItems.docx");
            //ExEnd

            // Empty the collection
            dropDownItems.Clear();
            Assert.AreEqual(0, dropDownItems.Count);
        }

        //ExStart
        //ExFor:Fields.FieldAsk
        //ExFor:Fields.FieldAsk.BookmarkName
        //ExFor:Fields.FieldAsk.DefaultResponse
        //ExFor:Fields.FieldAsk.PromptOnceOnMailMerge
        //ExFor:Fields.FieldAsk.PromptText
        //ExFor:FieldOptions.UserPromptRespondent
        //ExFor:IFieldUserPromptRespondent
        //ExSummary:Shows how to create an ASK field and set its properties.
        [Test]
        public void FieldAsk()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Place a field where the response to our ASK field will be placed
            FieldRef fieldRef = (FieldRef)builder.InsertField(FieldType.FieldRef, true);
            fieldRef.BookmarkName = "MyAskField";
            builder.Writeln();

            // Insert the ASK field and edit its properties, making sure to reference our REF field
            FieldAsk fieldAsk = (FieldAsk)builder.InsertField(FieldType.FieldAsk, true);
            fieldAsk.BookmarkName = "MyAskField";
            fieldAsk.PromptText = "Please provide a response for this ASK field";
            fieldAsk.DefaultResponse = "Response from within the field.";
            fieldAsk.PromptOnceOnMailMerge = true;
            builder.Writeln();

            // ASK fields apply the default response to their respective REF fields during a mail merge
            System.Data.DataTable table = new System.Data.DataTable("My Table");
            table.Columns.Add("Column 1");
            table.Rows.Add("Row 1");
            table.Rows.Add("Row 2");

            FieldMergeField fieldMergeField = (FieldMergeField)builder.InsertField(FieldType.FieldMergeField, true);
            fieldMergeField.FieldName = "Column 1";

            // We can modify or override the default response in our ASK fields with a custom prompt responder, which will take place during a mail merge
            doc.FieldOptions.UserPromptRespondent = new MyPromptRespondent();
            doc.MailMerge.Execute(table);

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Fields.AskField.docx");

            Assert.AreEqual(
                " ASK  MyAskField \"Please provide a response for this ASK field\" \\d \"Response from within the field.\" \\o",
                fieldAsk.GetFieldCode());

            Assert.AreEqual("MyAskField", fieldAsk.BookmarkName); //ExSkip
            Assert.AreEqual("Please provide a response for this ASK field", fieldAsk.PromptText); // ExSkip
            Assert.AreEqual("Response from within the field.", fieldAsk.DefaultResponse); //ExSkip
            Assert.AreEqual(true, fieldAsk.PromptOnceOnMailMerge); //ExSkip
        }

        /// <summary>
        /// IFieldUserPromptRespondent implementation that appends a line to the default response of an ASK field during a mail merge
        /// </summary>
        private class MyPromptRespondent : IFieldUserPromptRespondent
        {
            public string Respond(string promptText, string defaultResponse)
            {
                return "Response from MyPromptRespondent. " + defaultResponse;
            }
        }
        //ExEnd

        [Test]
        public void FieldAdvance()
        {
            //ExStart
            //ExFor:Fields.FieldAdvance
            //ExFor:Fields.FieldAdvance.DownOffset
            //ExFor:Fields.FieldAdvance.HorizontalPosition
            //ExFor:Fields.FieldAdvance.LeftOffset
            //ExFor:Fields.FieldAdvance.RightOffset
            //ExFor:Fields.FieldAdvance.UpOffset
            //ExFor:Fields.FieldAdvance.VerticalPosition
            //ExSummary:Shows how to insert an advance field and edit its properties. 
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("This text is in its normal place.");
            // Create an advance field using document builder
            FieldAdvance field = (FieldAdvance)builder.InsertField(FieldType.FieldAdvance, true);

            builder.Write("This text is moved up and to the right.");

            Assert.AreEqual(FieldType.FieldAdvance, field.Type);
            Assert.AreEqual(" ADVANCE ", field.GetFieldCode());
            // The second text that the builder added will now be moved
            field.RightOffset = "5";
            field.UpOffset = "5";

            Assert.AreEqual(" ADVANCE  \\r 5 \\u 5", field.GetFieldCode());
            // If we want to move text in the other direction, and try do that by using negative values for the above field members, we will get an error in our document
            // Instead, we need to specify a positive value for the opposite respective field directional variable
            field = (FieldAdvance)builder.InsertField(FieldType.FieldAdvance, true);
            field.DownOffset = "5";
            field.LeftOffset = "100";

            Assert.AreEqual(" ADVANCE  \\d 5 \\l 100", field.GetFieldCode());
            // We are still on one paragraph
            Assert.AreEqual(1, doc.FirstSection.Body.Paragraphs.Count);
            // Since we're setting horizontal and vertical positions next, we need to end the paragraph so the previous line does not get moved with the next one
            builder.Writeln("This text is moved down and to the left, overlapping the previous text.");
            // This time we can also use negative values 
            field = (FieldAdvance)builder.InsertField(FieldType.FieldAdvance, true);
            field.HorizontalPosition = "-100";
            field.VerticalPosition = "200";

            Assert.AreEqual(" ADVANCE  \\x -100 \\y 200", field.GetFieldCode());

            builder.Write("This text is in a custom position.");

            doc.Save(ArtifactsDir + "Field.Advance.docx");
            //ExEnd
        }


        [Test]
        public void FieldAddressBlock()
        {
            //ExStart
            //ExFor:Fields.FieldAddressBlock.ExcludedCountryOrRegionName
            //ExFor:Fields.FieldAddressBlock.FormatAddressOnCountryOrRegion
            //ExFor:Fields.FieldAddressBlock.IncludeCountryOrRegionName
            //ExFor:Fields.FieldAddressBlock.LanguageId
            //ExFor:Fields.FieldAddressBlock.NameAndAddressFormat
            //ExSummary:Shows how to build a field address block.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Use a document builder to insert a field address block
            FieldAddressBlock field = (FieldAddressBlock)builder.InsertField(FieldType.FieldAddressBlock, true);

            // Initially our field is an empty address block field with null attributes
            Assert.AreEqual(" ADDRESSBLOCK ", field.GetFieldCode());

            // Setting this to "2" will cause all countries/regions to be included, unless it is the one specified in the ExcludedCountryOrRegionName attribute
            field.IncludeCountryOrRegionName = "2";
            field.FormatAddressOnCountryOrRegion = true;
            field.ExcludedCountryOrRegionName = "United States";

            // Specify our own name and address format
            field.NameAndAddressFormat = "<Title> <Forename> <Surname> <Address Line 1> <Region> <Postcode> <Country>";

            // By default, the language ID will be set to that of the first character of the document
            // In this case we will specify it to be English
            field.LanguageId = "1033";

            // Our field code has changed according to the attribute values that we set
            Assert.AreEqual(
                " ADDRESSBLOCK  \\c 2 \\d \\e \"United States\" \\f \"<Title> <Forename> <Surname> <Address Line 1> <Region> <Postcode> <Country>\" \\l 1033",
                field.GetFieldCode());

            //ExEnd
            Assert.AreEqual("2", field.IncludeCountryOrRegionName);
            Assert.AreEqual(true, field.FormatAddressOnCountryOrRegion);
            Assert.AreEqual("United States", field.ExcludedCountryOrRegionName);
            Assert.AreEqual("<Title> <Forename> <Surname> <Address Line 1> <Region> <Postcode> <Country>",
                field.NameAndAddressFormat);
            Assert.AreEqual("1033", field.LanguageId);
        }

        //ExStart
        //ExFor:FieldCollection
        //ExFor:FieldCollection.Clear
        //ExFor:FieldCollection.Count
        //ExFor:FieldCollection.GetEnumerator
        //ExFor:FieldCollection.Item(Int32)
        //ExFor:FieldCollection.Remove(Field)
        //ExFor:FieldCollection.Remove(FieldStart)
        //ExFor:FieldCollection.RemoveAt(Int32)
        //ExFor:FieldEnd
        //ExFor:FieldEnd.Accept(DocumentVisitor)
        //ExFor:FieldEnd.HasSeparator
        //ExSummary:Shows how to work with a document's field collection.
        [Test] //ExSkip
        public void FieldCollection()
        {
            // Open a document that has fields
            Document doc = new Document(MyDir + "Document.ContainsFields.docx");

            // Get the collection that contains all the fields in a document
            FieldCollection fields = doc.Range.Fields;
            Assert.AreEqual(5, fields.Count);

            // Iterate over the field collection and print contents and type of every field using a custom visitor implementation
            FieldVisitor fieldVisitor = new FieldVisitor();

            using (IEnumerator<Field> fieldEnumerator = fields.GetEnumerator())
            {
                while (fieldEnumerator.MoveNext())
                {
                    if (fieldEnumerator.Current != null)
                    {
                        fieldEnumerator.Current.Start.Accept(fieldVisitor);
                        fieldEnumerator.Current.End.Accept(fieldVisitor);
                    }
                    else
                    {
                        Console.WriteLine("There are no fields in the document.");
                    }
                }
            }

            Console.WriteLine(fieldVisitor.GetText());

            // Get a field to remove itself
            fields[0].Remove();
            Assert.AreEqual(4, fields.Count);

            // Remove a field by reference
            Field lastField = fields[3];
            fields.Remove(lastField);
            Assert.AreEqual(3, fields.Count);

            // Remove a field by index
            fields.RemoveAt(2);
            Assert.AreEqual(2, fields.Count);

            // Remove all fields from the document
            fields.Clear();
            Assert.AreEqual(0, fields.Count);
        }

        /// <summary>
        /// Document visitor implementation that prints field info
        /// </summary>
        public class FieldVisitor : DocumentVisitor
        {
            public FieldVisitor()
            {
                mBuilder = new StringBuilder();
            }

            /// <summary>
            /// Gets the plain text of the document that was accumulated by the visitor.
            /// </summary>
            public String GetText()
            {
                return mBuilder.ToString();
            }

            /// <summary>
            /// Called when a FieldStart node is encountered in the document.
            /// </summary>
            public override VisitorAction VisitFieldStart(FieldStart fieldStart)
            {
                mBuilder.AppendLine("Found field: " + fieldStart.FieldType);
                mBuilder.AppendLine("\tField code: " + fieldStart.GetField().GetFieldCode());
                mBuilder.AppendLine("\tCurrent result: " + fieldStart.GetField().Result);

                return VisitorAction.Continue;
            }

            /// <summary>
            /// Called when a FieldEnd node is encountered in the document.
            /// </summary>
            public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
            {
                if (fieldEnd.HasSeparator)
                    mBuilder.AppendLine("\tSeparator: " + fieldEnd.GetField().Separator.GetText());

                return VisitorAction.Continue;
            }

            private readonly StringBuilder mBuilder;
        }
        //ExEnd

        [Test]
        public void FieldCompare()
        {
            //ExStart
            //ExFor:FieldCompare
            //ExFor:FieldCompare.ComparisonOperator
            //ExFor:FieldCompare.LeftExpression
            //ExFor:FieldCompare.RightExpression
            //ExSummary:Shows how to insert a field that compares expressions.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a compare field using a document builder
            FieldCompare field = (FieldCompare)builder.InsertField(FieldType.FieldCompare, true);

            // Construct a comparison statement
            field.LeftExpression = "3";
            field.ComparisonOperator = "<";
            field.RightExpression = "2";

            // The compare field will print a "0" or "1" depending on the truth of its statement
            // The result of this statement is false, so a "0" will be show up in the document
            Assert.AreEqual(" COMPARE  3 < 2", field.GetFieldCode());

            builder.Writeln();

            // Here a "1" will show up, because the statement is true
            field = (FieldCompare)builder.InsertField(FieldType.FieldCompare, true);
            field.LeftExpression = "5";
            field.ComparisonOperator = "=";
            field.RightExpression = "2 + 3";

            Assert.AreEqual(" COMPARE  5 = \"2 + 3\"", field.GetFieldCode());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Compare.docx");
            //ExEnd
        }

        [Test]
        public void FieldIf()
        {
            //ExStart
            //ExFor:FieldIf
            //ExFor:FieldIf.ComparisonOperator
            //ExFor:FieldIf.EvaluateCondition
            //ExFor:FieldIf.FalseText
            //ExFor:FieldIf.LeftExpression
            //ExFor:FieldIf.RightExpression
            //ExFor:FieldIf.TrueText
            //ExFor:FieldIfComparisonResult
            //ExSummary:Shows how to insert an if field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("Statement 1: ");

            // Use document builder to insert an if field
            FieldIf fieldIf = (FieldIf)builder.InsertField(FieldType.FieldIf, true);

            // The if field will output either the TrueText or FalseText string into the document, depending on the truth of the statement
            // In this case, "0 = 1" is incorrect, so the output will be "False"
            fieldIf.LeftExpression = "0";
            fieldIf.ComparisonOperator = "=";
            fieldIf.RightExpression = "1";
            fieldIf.TrueText = "True";
            fieldIf.FalseText = "False";

            Assert.AreEqual(" IF  0 = 1 True False", fieldIf.GetFieldCode());
            Assert.AreEqual(FieldIfComparisonResult.False, fieldIf.EvaluateCondition());

            // This time, the statement is correct, so the output will be "True"
            builder.Write("\nStatement 2: ");
            fieldIf = (FieldIf)builder.InsertField(FieldType.FieldIf, true);
            fieldIf.LeftExpression = "5";
            fieldIf.ComparisonOperator = "=";
            fieldIf.RightExpression = "2 + 3";
            fieldIf.TrueText = "True";
            fieldIf.FalseText = "False";

            Assert.AreEqual(" IF  5 = \"2 + 3\" True False", fieldIf.GetFieldCode());
            Assert.AreEqual(FieldIfComparisonResult.True, fieldIf.EvaluateCondition());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.If.docx");
            //ExEnd
        }

        [Test]
        public void FieldAutoNum()
        {
            //ExStart
            //ExFor:FieldAutoNum
            //ExFor:FieldAutoNum.SeparatorCharacter
            //ExSummary:Shows how to number paragraphs using autonum fields.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // The two fields we insert here will be automatically numbered 1 and 2
            builder.InsertField(FieldType.FieldAutoNum, true);
            builder.Writeln("\tParagraph 1.");
            builder.InsertField(FieldType.FieldAutoNum, true);
            builder.Writeln("\tParagraph 2.");

            foreach (Field field in doc.Range.Fields)
            {
                if (field.Type == FieldType.FieldAutoNum)
                {
                    // Leaving the FieldAutoNum.SeparatorCharacter field null will set the separator character to '.' by default
                    Assert.IsNull(((FieldAutoNum)field).SeparatorCharacter);

                    // The first character of the string entered here will be used as the separator character
                    ((FieldAutoNum)field).SeparatorCharacter = ":";

                    Assert.AreEqual(" AUTONUM  \\s :", field.GetFieldCode());
                }
            }

            doc.Save(ArtifactsDir + "Field.AutoNum.docx");
            //ExEnd
        }

        //ExStart
        //ExFor:FieldAutoNumLgl
        //ExFor:FieldAutoNumLgl.RemoveTrailingPeriod
        //ExFor:FieldAutoNumLgl.SeparatorCharacter
        //ExSummary:Shows how to organize a document using autonum legal fields
        [Test] //ExSkip
        public void FieldAutoNumLgl()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // This string will be our paragraph text that
            string loremIpsum =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                "\nUt enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. ";

            // In this case our autonum legal field will number our first paragraph as "1."
            InsertNumberedClause(builder, "\tHeading 1", loremIpsum, StyleIdentifier.Heading1);

            // Our heading style number will be 1 again, so this field will keep counting headings at a heading level of 1
            InsertNumberedClause(builder, "\tHeading 2", loremIpsum, StyleIdentifier.Heading1);

            // Our heading style is 2, setting the paragraph numbering depth to 2, setting this field's value to "2.1."
            InsertNumberedClause(builder, "\tHeading 3", loremIpsum, StyleIdentifier.Heading2);

            // Our heading style is 3, so we are going deeper again to "2.1.1."
            InsertNumberedClause(builder, "\tHeading 4", loremIpsum, StyleIdentifier.Heading3);

            // Our heading style is 2, and the next field number at that level is "2.2."
            InsertNumberedClause(builder, "\tHeading 5", loremIpsum, StyleIdentifier.Heading2);

            foreach (Field field in doc.Range.Fields)
            {
                if (field.Type == FieldType.FieldAutoNumLegal)
                {
                    // By default the separator will appear as "." in the document but here it is null
                    Assert.IsNull(((FieldAutoNumLgl)field).SeparatorCharacter);

                    // Change the separator character and remove trailing separators
                    ((FieldAutoNumLgl)field).SeparatorCharacter = ":";
                    ((FieldAutoNumLgl)field).RemoveTrailingPeriod = true;
                    Assert.AreEqual(" AUTONUMLGL  \\s : \\e", field.GetFieldCode());
                }
            }

            doc.Save(ArtifactsDir + "Field.AutoNumLegal.docx");
        }

        /// <summary>
        /// Get a document builder to insert a clause numbered by an autonum legal field
        /// </summary>
        private void InsertNumberedClause(DocumentBuilder builder, string heading, string contents, StyleIdentifier headingStyle)
        {
            // This legal field will automatically number our clauses, taking heading style level into account
            builder.InsertField(FieldType.FieldAutoNumLegal, true);
            builder.CurrentParagraph.ParagraphFormat.StyleIdentifier = headingStyle;
            builder.Writeln(heading);

            // This text will belong to the auto num legal field above it
            // It will collapse when the arrow next to the corresponding autonum legal field is clicked in MS Word
            builder.CurrentParagraph.ParagraphFormat.StyleIdentifier = StyleIdentifier.BodyText;
            builder.Writeln(contents);
        }
        //ExEnd

        [Test]
        public void FieldAutoNumOut()
        {
            //ExStart
            //ExFor:FieldAutoNumOut
            //ExSummary:Shows how to number paragraphs using autonum outline fields.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // The two fields that we insert here will be numbered 1 and 2
            builder.InsertField(FieldType.FieldAutoNumOutline, true);
            builder.Writeln("\tParagraph 1.");
            builder.InsertField(FieldType.FieldAutoNumOutline, true);
            builder.Writeln("\tParagraph 2.");

            foreach (Field field in doc.Range.Fields)
            {
                if (field.Type == FieldType.FieldAutoNumOutline)
                {
                    Assert.AreEqual(" AUTONUMOUT ", field.GetFieldCode());
                }
            }

            doc.Save(ArtifactsDir + "Field.AutoNumOut.docx");
            //ExEnd
        }

        [Test]
        public void FieldAutoText()
        {
            //ExStart
            //ExFor:Fields.FieldAutoText
            //ExFor:FieldAutoText.EntryName
            //ExFor:FieldOptions.BuiltInTemplatesPaths
            //ExFor:FieldGlossary
            //ExFor:FieldGlossary.EntryName
            //ExSummary:Shows how to insert a building block into a document and display it with AUTOTEXT and GLOSSARY fields. 
            Document doc = new Document();

            // Create a glossary document and add an AutoText building block
            doc.GlossaryDocument = new GlossaryDocument();
            BuildingBlock buildingBlock = new BuildingBlock(doc.GlossaryDocument);
            buildingBlock.Name = "MyBlock";
            buildingBlock.Gallery = BuildingBlockGallery.AutoText;
            buildingBlock.Category = "General";
            buildingBlock.Description = "MyBlock description";
            buildingBlock.Behavior = BuildingBlockBehavior.Paragraph;
            doc.GlossaryDocument.AppendChild(buildingBlock);

            // Create a source and add it as text content to our building block
            Document buildingBlockSource = new Document();
            DocumentBuilder buildingBlockSourceBuilder = new DocumentBuilder(buildingBlockSource);
            buildingBlockSourceBuilder.Writeln("Hello World!");

            Node buildingBlockContent = doc.GlossaryDocument.ImportNode(buildingBlockSource.FirstSection, true);
            buildingBlock.AppendChild(buildingBlockContent);

            // Create an advance field using document builder
            DocumentBuilder builder = new DocumentBuilder(doc);
            FieldAutoText fieldAutoText = (FieldAutoText)builder.InsertField(FieldType.FieldAutoText, true);

            // Refer to our building block by name
            fieldAutoText.EntryName = "MyBlock";

            Assert.AreEqual(" AUTOTEXT  MyBlock", fieldAutoText.GetFieldCode());

            // Put additional templates here
            doc.FieldOptions.BuiltInTemplatesPaths = new[] { MyDir + "Document.BusinessBrochureTemplate.dotx" };

            // We can also display our building block with a GLOSSARY field
            FieldGlossary fieldGlossary = (FieldGlossary)builder.InsertField(FieldType.FieldGlossary, true);
            fieldGlossary.EntryName = "MyBlock";

            Assert.AreEqual(" GLOSSARY  MyBlock", fieldGlossary.GetFieldCode());

            // The text content of our building block will be visible in the output
			doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.AutoText.dotx");
            //ExEnd
        }

        //ExStart
        //ExFor:Fields.FieldAutoTextList
        //ExFor:Fields.FieldAutoTextList.EntryName
        //ExFor:Fields.FieldAutoTextList.ListStyle
        //ExFor:Fields.FieldAutoTextList.ScreenTip
        //ExSummary:Shows how to use an AutoTextList field to select from a list of AutoText entries.
        [Test] //ExSkip
        public void FieldAutoTextList()
        {
            Document doc = new Document();

            // Create a glossary document and populate it with auto text entries that our auto text list will let us select from
            doc.GlossaryDocument = new GlossaryDocument();
            AppendAutoTextEntry(doc.GlossaryDocument, "AutoText 1", "Contents of AutoText 1");
            AppendAutoTextEntry(doc.GlossaryDocument, "AutoText 2", "Contents of AutoText 2");
            AppendAutoTextEntry(doc.GlossaryDocument, "AutoText 3", "Contents of AutoText 3");

            // Insert an auto text list using a document builder and change its properties
            DocumentBuilder builder = new DocumentBuilder(doc);
            FieldAutoTextList field = (FieldAutoTextList)builder.InsertField(FieldType.FieldAutoTextList, true);
            field.EntryName = "Right click here to pick an AutoText block"; // This is the text that will be visible in the document
            field.ListStyle = "Heading 1";
            field.ScreenTip = "Hover tip text for AutoTextList goes here";

            Assert.AreEqual("Right click here to pick an AutoText block", field.EntryName); //ExSkip
            Assert.AreEqual("Heading 1", field.ListStyle); //ExSkip
            Assert.AreEqual("Hover tip text for AutoTextList goes here", field.ScreenTip); //ExSkip
            Assert.AreEqual(" AUTOTEXTLIST  \"Right click here to pick an AutoText block\" " +
                            "\\s \"Heading 1\" " +
                            "\\t \"Hover tip text for AutoTextList goes here\"", field.GetFieldCode());

            doc.Save(ArtifactsDir + "Field.AutoTextList.dotx");
        }

        /// <summary>
        /// Create an AutoText entry and add it to a glossary document
        /// </summary>
        private static void AppendAutoTextEntry(GlossaryDocument glossaryDoc, string name, string contents)
        {
            // Create building block and set it up as an auto text entry
            BuildingBlock buildingBlock = new BuildingBlock(glossaryDoc);
            buildingBlock.Name = name;
            buildingBlock.Gallery = BuildingBlockGallery.AutoText;
            buildingBlock.Category = "General";
            buildingBlock.Behavior = BuildingBlockBehavior.Paragraph;

            // Add content to the building block
            Section section = new Section(glossaryDoc);
            section.AppendChild(new Body(glossaryDoc));
            section.Body.AppendParagraph(contents);
            buildingBlock.AppendChild(section);

            // Add auto text entry to glossary document
            glossaryDoc.AppendChild(buildingBlock);
        }
        //ExEnd

        [Test]
        public void FieldGreetingLine()
        {
            //ExStart
            //ExFor:FieldGreetingLine
            //ExFor:FieldGreetingLine.AlternateText
            //ExFor:FieldGreetingLine.GetFieldNames
            //ExFor:FieldGreetingLine.LanguageId
            //ExFor:FieldGreetingLine.NameFormat
            //ExSummary:Shows how to insert a GREETINGLINE field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a custom greeting field with document builder, and also some content
            FieldGreetingLine fieldGreetingLine = (FieldGreetingLine)builder.InsertField(FieldType.FieldGreetingLine, true);
            builder.Writeln("\n\n\tThis is your custom greeting, created programmatically using Aspose Words!");

            // This array contains strings that correspond to column names in the data table that we will mail merge into our document
            Assert.AreEqual(0, fieldGreetingLine.GetFieldNames().Length);

            // To populate that array, we need to specify a format for our greeting line
            fieldGreetingLine.NameFormat = "<< _BEFORE_ Dear >><< _TITLE0_ >><< _LAST0_ >><< _AFTER_ ,>> ";

            // In this case, our greeting line's field names array now has "Courtesy Title" and "Last Name"
            Assert.AreEqual(2, fieldGreetingLine.GetFieldNames().Length);

            // This string will cover any cases where the data in the data table is incorrect by substituting the malformed name with a string
            fieldGreetingLine.AlternateText = "Sir or Madam";

            // We can set the language ID here too
            fieldGreetingLine.LanguageId = "1033";

            Assert.AreEqual(" GREETINGLINE  \\f \"<< _BEFORE_ Dear >><< _TITLE0_ >><< _LAST0_ >><< _AFTER_ ,>> \" \\e \"Sir or Madam\" \\l 1033", fieldGreetingLine.GetFieldCode());

            // Create a source table for our mail merge that has columns that our greeting line will look for
            System.Data.DataTable table = new System.Data.DataTable("Employees");
            table.Columns.Add("Courtesy Title");
            table.Columns.Add("First Name");
            table.Columns.Add("Last Name");
            table.Rows.Add("Mr.", "John", "Doe");
            table.Rows.Add("Mrs.", "Jane", "Cardholder");
            table.Rows.Add("", "No", "Name"); // This row has an invalid value in the Courtesy Title column, so our greeting will default to the alternate text

            doc.MailMerge.Execute(table);

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.GreetingLine.docx");
            //ExEnd
        }

        [Test]
        public void FieldListNum()
        {
            //ExStart
            //ExFor:FieldListNum
            //ExFor:FieldListNum.HasListName
            //ExFor:FieldListNum.ListLevel
            //ExFor:FieldListNum.ListName
            //ExFor:FieldListNum.StartingNumber
            //ExSummary:Shows how to number paragraphs with LISTNUM fields.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a list num field using a document builder
            FieldListNum fieldListNum = (FieldListNum)builder.InsertField(FieldType.FieldListNum, true);

            // Lists start counting at 1 by default, but we can change this number at any time
            // In this case, we'll do a zero-based count
            fieldListNum.StartingNumber = "0";
            builder.Writeln("Paragraph 1");

            // Placing several list num fields in one paragraph increases the list level instead of the current number, in this case resulting in "1)a)i)", list level 3
            builder.InsertField(FieldType.FieldListNum, true);
            builder.InsertField(FieldType.FieldListNum, true);
            builder.InsertField(FieldType.FieldListNum, true);
            builder.Writeln("Paragraph 2");

            // The list level resets with new paragraphs, so to keep counting at a desired list level, we need to set the ListLevel property accordingly
            fieldListNum = (FieldListNum)builder.InsertField(FieldType.FieldListNum, true);
            fieldListNum.ListLevel = "3";
            builder.Writeln("Paragraph 3");

            fieldListNum = (FieldListNum)builder.InsertField(FieldType.FieldListNum, true);

            // Setting this property to this particular value will emulate the AUTONUMOUT field
            fieldListNum.ListName = "OutlineDefault";
            Assert.IsTrue(fieldListNum.HasListName);

            // Start counting from 1
            fieldListNum.StartingNumber = "1";
            builder.Writeln("Paragraph 4");

            // Our fields keep track of the count automatically, but the ListName needs to be set with each new field
            fieldListNum = (FieldListNum)builder.InsertField(FieldType.FieldListNum, true);
            fieldListNum.ListName = "OutlineDefault";
            builder.Writeln("Paragraph 5");

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FieldListNum.docx");
            //ExEnd
        }

        [Test]
        public void MergeField()
        {
            //ExStart
            //ExFor:FieldMergeField
            //ExFor:FieldMergeField.FieldName
            //ExFor:FieldMergeField.FieldNameNoPrefix
            //ExFor:FieldMergeField.IsMapped
            //ExFor:FieldMergeField.IsVerticalFormatting
            //ExFor:FieldMergeField.TextAfter
            //ExSummary:Shows how to use MERGEFIELD fields to perform a mail merge.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create data source for our merge fields
            System.Data.DataTable table = new System.Data.DataTable("Employees");
            table.Columns.Add("Courtesy Title");
            table.Columns.Add("First Name");
            table.Columns.Add("Last Name");
            table.Rows.Add("Mr.", "John", "Doe");
            table.Rows.Add("Mrs.", "Jane", "Cardholder");

            // Insert a merge field that corresponds to one of our columns and put text before and after it
            FieldMergeField fieldMergeField = (FieldMergeField)builder.InsertField(FieldType.FieldMergeField, true);
            fieldMergeField.FieldName = "Courtesy Title";
            fieldMergeField.IsMapped = true;
            fieldMergeField.IsVerticalFormatting = false;
            fieldMergeField.TextBefore = "Dear ";
            fieldMergeField.TextAfter = " ";

            // Insert another merge field for another column
            // We don't need to use every column to perform a mail merge
            fieldMergeField = (FieldMergeField)builder.InsertField(FieldType.FieldMergeField, true);
            fieldMergeField.FieldName = "Last Name";
            fieldMergeField.TextAfter = ":";

            doc.UpdateFields();
            doc.MailMerge.Execute(table);
            doc.Save(ArtifactsDir + "Field.MergeField.docx");
            //ExEnd
        }

        //ExStart
        //ExFor:FormField.Accept(DocumentVisitor)
        //ExFor:FormField.CalculateOnExit
        //ExFor:FormField.CheckBoxSize
        //ExFor:FormField.Checked
        //ExFor:FormField.Default
        //ExFor:FormField.DropDownItems
        //ExFor:FormField.DropDownSelectedIndex
        //ExFor:FormField.Enabled
        //ExFor:FormField.EntryMacro
        //ExFor:FormField.ExitMacro
        //ExFor:FormField.HelpText
        //ExFor:FormField.IsCheckBoxExactSize
        //ExFor:FormField.MaxLength
        //ExFor:FormField.OwnHelp
        //ExFor:FormField.OwnStatus
        //ExFor:FormField.SetTextInputValue(Object)
        //ExFor:FormField.StatusText
        //ExFor:FormField.TextInputDefault
        //ExFor:FormField.TextInputFormat
        //ExFor:FormField.TextInputType
        //ExFor:FormFieldCollection.Clear
        //ExFor:FormFieldCollection.Count
        //ExFor:FormFieldCollection.GetEnumerator
        //ExFor:FormFieldCollection.Item(Int32)
        //ExFor:FormFieldCollection.Item(String)
        //ExFor:FormFieldCollection.Remove(String)
        //ExFor:FormFieldCollection.RemoveAt(Int32)
        //ExSummary:Shows how insert different kinds of form fields into a document and process them with a visitor implementation.
        [Test] //ExSkip
        public void FormField()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Use a document builder to insert a combo box
            FormField comboBox = builder.InsertComboBox("MyComboBox", new[] { "One", "Two", "Three" }, 0);
            comboBox.CalculateOnExit = true;
            Assert.AreEqual(3, comboBox.DropDownItems.Count);
            Assert.AreEqual(0, comboBox.DropDownSelectedIndex);
            Assert.AreEqual(true, comboBox.Enabled);

            builder.Writeln();

            // Use a document builder to insert a check box
            FormField checkBox = builder.InsertCheckBox("MyCheckBox", false, 50);
            checkBox.IsCheckBoxExactSize = true;
            checkBox.HelpText = "Right click to check this box";
            checkBox.OwnHelp = true;
            checkBox.StatusText = "Checkbox status text";
            checkBox.OwnStatus = true;
            Assert.AreEqual(50.0d, checkBox.CheckBoxSize);
            Assert.AreEqual(false, checkBox.Checked);
            Assert.AreEqual(false, checkBox.Default);

            builder.Writeln();

            // Use a document builder to insert text input form field
            FormField textInput = builder.InsertTextInput("MyTextInput", TextFormFieldType.Regular, "", "Your text goes here", 50);
            Assert.AreEqual(3, doc.Range.Fields.Count);
            textInput.EntryMacro = "EntryMacro";
            textInput.ExitMacro = "ExitMacro";
            textInput.TextInputDefault = "Regular";
            textInput.TextInputFormat = "FIRST CAPITAL";
            textInput.SetTextInputValue("This value overrides the one we set during initialization");
            Assert.AreEqual(TextFormFieldType.Regular, textInput.TextInputType);
            Assert.AreEqual(50, textInput.MaxLength);

            // Get the collection of form fields that has accumulated in our document
            FormFieldCollection formFields = doc.Range.FormFields;
            Assert.AreEqual(3, formFields.Count);

            // Iterate over the collection with an enumerator, accepting a visitor with each form field
            FormFieldVisitor formFieldVisitor = new FormFieldVisitor();

            using (IEnumerator<FormField> fieldEnumerator = formFields.GetEnumerator())
            {
                while (fieldEnumerator.MoveNext())
                {
                    fieldEnumerator.Current.Accept(formFieldVisitor);
                }
            }

            Console.WriteLine(formFieldVisitor.GetText());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FormField.docx");
        }

        /// <summary>
        /// Visitor implementation that prints information about visited form fields. 
        /// </summary>
        public class FormFieldVisitor : DocumentVisitor
        {
            public FormFieldVisitor()
            {
                mBuilder = new StringBuilder();
            }

            /// <summary>
            /// Called when a FormField node is encountered in the document.
            /// </summary>
            public override VisitorAction VisitFormField(FormField formField)
            {
                AppendLine(formField.Type + ": \"" + formField.Name + "\"");
                AppendLine("\tStatus: " + (formField.Enabled ? "Enabled" : "Disabled"));
                AppendLine("\tHelp Text:  " + formField.HelpText);
                AppendLine("\tEntry macro name: " + formField.EntryMacro);
                AppendLine("\tExit macro name: " + formField.ExitMacro);

                switch (formField.Type)
                {
                    case FieldType.FieldFormDropDown:
                        AppendLine("\tDrop down items count: " + formField.DropDownItems.Count + ", default selected item index: " + formField.DropDownSelectedIndex);
                        AppendLine("\tDrop down items: " + string.Join(", ", formField.DropDownItems.ToArray()));
                        break;
                    case FieldType.FieldFormCheckBox:
                        AppendLine("\tCheckbox size: " + formField.CheckBoxSize);
                        AppendLine("\t" + "Checkbox is currently: " + (formField.Checked ? "checked, " : "unchecked, ") + "by default: " + (formField.Default ? "checked" : "unchecked"));
                        break;
                    case FieldType.FieldFormTextInput:
                        AppendLine("\tInput format: " + formField.TextInputFormat);
                        AppendLine("\tCurrent contents: " + formField.Result);
                        break;
                }

                // Let the visitor continue visiting other nodes.
                return VisitorAction.Continue;
            }

            /// <summary>
            /// Adds newline char-terminated text to the current output.
            /// </summary>
            private void AppendLine(string text)
            {
                mBuilder.Append(text + '\n');
            }

            /// <summary>
            /// Gets the plain text of the document that was accumulated by the visitor.
            /// </summary>
            public string GetText()
            {
                return mBuilder.ToString();
            }

            private readonly StringBuilder mBuilder;
        }
        //ExEnd

        //ExStart
        //ExFor:FieldToc
        //ExFor:FieldToc.BookmarkName
        //ExFor:FieldToc.CustomStyles
        //ExFor:FieldToc.EntrySeparator
        //ExFor:FieldToc.HeadingLevelRange
        //ExFor:FieldToc.HideInWebLayout
        //ExFor:FieldToc.InsertHyperlinks
        //ExFor:FieldToc.PageNumberOmittingLevelRange
        //ExFor:FieldToc.PreserveLineBreaks
        //ExFor:FieldToc.PreserveTabs
        //ExFor:FieldToc.UpdatePageNumbers
        //ExFor:FieldToc.UseParagraphOutlineLevel
        //ExFor:FieldOptions.CustomTocStyleSeparator
        //ExSummary:Shows how to insert a TOC and populate it with entries based on heading styles.
        [Test] //ExSkip
        public void FieldToc()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // The table of contents we will insert will accept entries that are only within the scope of this bookmark
            builder.StartBookmark("MyBookmark");

            // Insert a list num field using a document builder
            FieldToc fieldToc = (FieldToc)builder.InsertField(FieldType.FieldTOC, true);

            // Limit possible TOC entries to only those within the bookmark we name here
            fieldToc.BookmarkName = "MyBookmark";

            // Normally paragraphs with a "Heading n" style will be the only ones that will be added to a TOC as entries
            // We can set this attribute to include other styles, such as "Quote" and "Intense Quote" in this case
            fieldToc.CustomStyles = "Quote; 6; Intense Quote; 7";

            // Styles are normally separated by a comma (",") but we can use this property to set a custom delimiter
            doc.FieldOptions.CustomTocStyleSeparator = ";";

            // Filter out any headings that are outside this range
            fieldToc.HeadingLevelRange = "1-3";

            // Headings in this range won't display their page number in their TOC entry
            fieldToc.PageNumberOmittingLevelRange = "2-5";

            fieldToc.EntrySeparator = "-";
            fieldToc.InsertHyperlinks = true;
            fieldToc.HideInWebLayout = false;
            fieldToc.PreserveLineBreaks = true;
            fieldToc.PreserveTabs = true;
            fieldToc.UseParagraphOutlineLevel = false;

            InsertHeading(builder, "First entry", "Heading 1");
            builder.Writeln("Paragraph text.");
            InsertHeading(builder, "Second entry", "Heading 1");
            InsertHeading(builder, "Third entry", "Quote");
            InsertHeading(builder, "Fourth entry", "Intense Quote");

            // These two headings will have the page numbers omitted because they are within the "2-5" range
            InsertHeading(builder, "Fifth entry", "Heading 2");
            InsertHeading(builder, "Sixth entry", "Heading 3");

            // This entry will be omitted because "Heading 4" is outside of the "1-3" range we set earlier
            InsertHeading(builder, "Seventh entry", "Heading 4");

            builder.EndBookmark("MyBookmark");
            builder.Writeln("Paragraph text.");

            // This entry will be omitted because it is outside the bookmark specified by the TOC
            InsertHeading(builder, "Eighth entry", "Heading 1");

            Assert.AreEqual(" TOC  \\b MyBookmark \\t \"Quote; 6; Intense Quote; 7\" \\o 1-3 \\n 2-5 \\p - \\h \\x \\w", fieldToc.GetFieldCode());

            fieldToc.UpdatePageNumbers();
            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FieldTOC.docx");
        }

        /// <summary>
        /// Start a new page and insert a paragraph of a specified style
        /// </summary>
        public void InsertHeading(DocumentBuilder builder, string captionText, string styleName)
        {
            builder.InsertBreak(BreakType.PageBreak);
            string originalStyle = builder.ParagraphFormat.StyleName;
            builder.ParagraphFormat.Style = builder.Document.Styles[styleName];
            builder.Writeln(captionText);
            builder.ParagraphFormat.Style = builder.Document.Styles[originalStyle];
        }
        //ExEnd

        //ExStart
        //ExFor:FieldToc.EntryIdentifier
        //ExFor:FieldToc.EntryLevelRange
        //ExFor:FieldTC
        //ExFor:FieldTC.OmitPageNumber
        //ExFor:FieldTC.Text
        //ExFor:FieldTC.TypeIdentifier
        //ExFor:FieldTC.EntryLevel
        //ExSummary:Shows how to insert a TOC field and filter which TC fields end up as entries.
        [Test] //ExSkip
        public void FieldTocEntryIdentifier()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartBookmark("MyBookmark");

            // Insert a list num field using a document builder
            FieldToc fieldToc = (FieldToc)builder.InsertField(FieldType.FieldTOC, true);
            fieldToc.EntryIdentifier = "A";
            fieldToc.EntryLevelRange = "1-3";

            builder.InsertBreak(BreakType.PageBreak);

            // These two entries will appear in the table
            InsertTocEntry(builder, "TC field 1", "A", "1");
            InsertTocEntry(builder, "TC field 2", "A", "2");

            // These two entries will be omitted because of an incorrect type identifier
            InsertTocEntry(builder, "TC field 3", "B", "1");

            // ...and an out-of-range entry level
            InsertTocEntry(builder, "TC field 4", "A", "5");

            Assert.AreEqual(" TOC  \\f A \\l 1-3", fieldToc.GetFieldCode());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FieldTOC.TC.docx");
        }

        /// <summary>
        /// Insert a table of contents entry via a document builder
        /// </summary>
        public void InsertTocEntry(DocumentBuilder builder, string text, string typeIdentifier, string entryLevel)
        {
            FieldTC fieldTc = (FieldTC)builder.InsertField(FieldType.FieldTOCEntry, true);
            fieldTc.OmitPageNumber = true;
            fieldTc.Text = text;
            fieldTc.TypeIdentifier = typeIdentifier;
            fieldTc.EntryLevel = entryLevel;
        }
        //ExEnd

        //ExStart
        //ExFor:FieldToc.TableOfFiguresLabel
        //ExFor:FieldToc.CaptionlessTableOfFiguresLabel
        //ExFor:FieldToc.PrefixedSequenceIdentifier
        //ExFor:FieldToc.SequenceSeparator
        //ExSummary:Insert a TOC field and build the table with SEQ fields.
        [Test] //ExSkip
        public void FieldTocFigure()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a list num field using a document builder
            FieldToc fieldToc = (FieldToc)builder.InsertField(FieldType.FieldTOC, true);
            fieldToc.CaptionlessTableOfFiguresLabel = "Figures";
            fieldToc.PrefixedSequenceIdentifier = "ChapterNum";
            fieldToc.SequenceSeparator = ":";

            // By default, the table of contents 
            fieldToc.TableOfFiguresLabel = "Figure";
            builder.InsertBreak(BreakType.PageBreak);

            // These captions will have a sequence identifier that's the same as the table of figures label in our table of contents,
            // so the table of contents will pick them up
            InsertCaption(builder, "Prefix ", "ChapterNum");
            InsertCaption(builder, " Figure ", "Figure");
            builder.Writeln("\nMy paragraph contents.");
            InsertCaption(builder, "Prefix ", "ChapterNum");
            InsertCaption(builder, " Figure ", "Figure");
            builder.Writeln("\nMy paragraph contents.");

            // This will start a new count and won't be picked up by our table of contents
            InsertCaption(builder, "Figure ", "OtherFigureSequence");
            builder.Writeln("My paragraph contents.");

            Assert.AreEqual(" TOC  \\a Figures \\s ChapterNum \\d : \\c Figure", fieldToc.GetFieldCode());

            fieldToc.UpdatePageNumbers();
            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FieldTOC.SEQ.docx");
        }

        /// <summary>
        /// Insert a sequence field with preceding text and a specified sequence identifier
        /// </summary>
        public void InsertCaption(DocumentBuilder builder, string precedingText, string sequenceIdentifier)
        {
            builder.Write(precedingText);
            FieldSeq caption = (FieldSeq)builder.InsertField(FieldType.FieldSequence, false);
            caption.SequenceIdentifier = sequenceIdentifier;
        }
        //ExEnd

        [Test]
        [Ignore("WORDSNET-13854")]
        public void FieldCitation()
        {
            //ExStart
            //ExFor:FieldCitation
            //ExFor:FieldCitation.AnotherSourceTag
            //ExFor:FieldCitation.FormatLanguageId
            //ExFor:FieldCitation.PageNumber
            //ExFor:FieldCitation.Prefix
            //ExFor:FieldCitation.SourceTag
            //ExFor:FieldCitation.Suffix
            //ExFor:FieldCitation.SuppressAuthor
            //ExFor:FieldCitation.SuppressTitle
            //ExFor:FieldCitation.SuppressYear
            //ExFor:FieldCitation.VolumeNumber
            //ExFor:FieldBibliography
            //ExFor:FieldBibliography.FormatLanguageId
            //ExSummary:Shows how to work with CITATION and BIBLIOGRAPHY fields.
            // Open a document that has bibliographical sources
            Document doc = new Document(MyDir + "Document.HasBibliography.docx");

            // Add text that we can cite
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write("Text to be cited with one source.");

            // Create a citation field using the document builder
            FieldCitation field = (FieldCitation)builder.InsertField(FieldType.FieldCitation, true);

            // A simple citation can have just the page number and author's name
            field.SourceTag = "Book1"; // We refer to sources using their tag names
            field.PageNumber = "85";
            field.SuppressAuthor = false;
            field.SuppressTitle = true;
            field.SuppressYear = true;

            Assert.AreEqual(" CITATION  Book1 \\p 85 \\t \\y", field.GetFieldCode());

            // We can make a more detailed citation and make it cite 2 sources
            builder.Write("Text to be cited with two sources.");
            field = (FieldCitation)builder.InsertField(FieldType.FieldCitation, true);
            field.SourceTag = "Book1";
            field.AnotherSourceTag = "Book2";
            field.FormatLanguageId = "en-US";
            field.PageNumber = "19";
            field.Prefix = "Prefix ";
            field.Suffix = " Suffix";
            field.SuppressAuthor = false;
            field.SuppressTitle = false;
            field.SuppressYear = false;
            field.VolumeNumber = "VII";

            Assert.AreEqual(" CITATION  Book1 \\m Book2 \\l en-US \\p 19 \\f \"Prefix \" \\s \" Suffix\" \\v VII", field.GetFieldCode());

            // Insert a new page which will contain our bibliography
            builder.InsertBreak(BreakType.PageBreak);

            // All our sources can be displayed using a BIBLIOGRAPHY field
            FieldBibliography fieldBibliography = (FieldBibliography)builder.InsertField(FieldType.FieldBibliography, true);
            fieldBibliography.FormatLanguageId = "1124";

            Assert.AreEqual(" BIBLIOGRAPHY  \\l 1124", fieldBibliography.GetFieldCode());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Citation.docx");
            //ExEnd
        }

        [Test]
        public void FieldData()
        {
            //ExStart
            //ExFor:FieldData
            //ExSummary:Shows how to insert a data field into a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Use a document builder to insert a data field
            FieldData field = (FieldData)builder.InsertField(FieldType.FieldData, true);
            Assert.AreEqual(" DATA ", field.GetFieldCode());
            //ExEnd
        }

        [Test]
        public void FieldInclude()
        {
            //ExStart
            //ExFor:FieldInclude
            //ExFor:FieldInclude.BookmarkName
            //ExFor:FieldInclude.LockFields
            //ExFor:FieldInclude.SourceFullName
            //ExFor:FieldInclude.TextConverter
            //ExSummary:Shows how to create an INCLUDE field and set its properties.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add an INCLUDE field with document builder and import a portion of the document defined by a bookmark
            FieldInclude fieldInclude = (FieldInclude)builder.InsertField(FieldType.FieldInclude, true);
            fieldInclude.SourceFullName = MyDir + "Field.Include.Source.docx";
            fieldInclude.BookmarkName = "Source_paragraph_2";
            fieldInclude.LockFields = false;
            fieldInclude.TextConverter = "Microsoft Word";

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Include.docx");
            //ExEnd
        }

        [Test]
        [Ignore("WORDSNET-13854")]
        public void FieldDatabase()
        {
            //ExStart
            //ExFor:FieldDatabase
            //ExFor:FieldDatabase.Connection
            //ExFor:FieldDatabase.FileName
            //ExFor:FieldDatabase.FirstRecord
            //ExFor:FieldDatabase.FormatAttributes
            //ExFor:FieldDatabase.InsertHeadings
            //ExFor:FieldDatabase.InsertOnceOnMailMerge
            //ExFor:FieldDatabase.LastRecord
            //ExFor:FieldDatabase.Query
            //ExFor:FieldDatabase.TableFormat
            //ExSummary:Shows how to extract data from a database and insert it as a field into a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Use a document builder to insert a database field
            FieldDatabase field = (FieldDatabase)builder.InsertField(FieldType.FieldDatabase, true);

            // Create a simple query that extracts one table from the database
            field.FileName = MyDir + @"Database\Northwind.mdb";
            field.Connection = "DSN=MS Access Databases";
            field.Query = "SELECT * FROM [Products]";

            // Insert another database field
            field = (FieldDatabase)builder.InsertField(FieldType.FieldDatabase, true);
            field.FileName = MyDir + @"Database\Northwind.mdb";
            field.Connection = "DSN=MS Access Databases";

            // This query will sort all the products by their gross sales in descending order
            field.Query =
                "SELECT [Products].ProductName, FORMAT(SUM([Order Details].UnitPrice * (1 - [Order Details].Discount) * [Order Details].Quantity), 'Currency') AS GrossSales " +
                "FROM([Products] " +
                "LEFT JOIN[Order Details] ON[Products].[ProductID] = [Order Details].[ProductID]) " +
                "GROUP BY[Products].ProductName " +
                "ORDER BY SUM([Order Details].UnitPrice* (1 - [Order Details].Discount) * [Order Details].Quantity) DESC";

            // You can use these variables instead of a LIMIT clause, to simplify your query
            // In this case we are taking the first 10 values of the result of our query
            field.FirstRecord = "1";
            field.LastRecord = "10";

            // The number we put here is the index of the format we want to use for our table
            // The list of table formats is in the "Table AutoFormat..." menu we find in MS Word when we create a data table field
            // Index "10" corresponds to the "Colorful 3" format
            field.TableFormat = "10";

            // This attribute decides which elements of the table format we picked above we incorporate into our table
            // The number we use is a sum of a combination of values corresponding to which elements we choose
            // 63 represents borders (1) + shading (2) + font (4) + colour (8) + autofit (16) + heading rows (32)
            field.FormatAttributes = "63";

            field.InsertHeadings = true;
            field.InsertOnceOnMailMerge = true;

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Database.docx");
            //ExEnd
        }

        [Test]
        public void FieldIncludePicture()
        {
            //ExStart
            //ExFor:FieldIncludePicture
            //ExFor:FieldIncludePicture.GraphicFilter
            //ExFor:FieldIncludePicture.IsLinked
            //ExFor:FieldIncludePicture.ResizeHorizontally
            //ExFor:FieldIncludePicture.ResizeVertically
            //ExFor:FieldIncludePicture.SourceFullName
            //ExFor:FieldImport
            //ExFor:FieldImport.GraphicFilter
            //ExFor:FieldImport.IsLinked
            //ExFor:FieldImport.SourceFullName
            //ExSummary:Shows how to insert images using IMPORT and INCLUDEPICTURE fields.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            FieldIncludePicture fieldIncludePicture = (FieldIncludePicture)builder.InsertField(FieldType.FieldIncludePicture, true);
            fieldIncludePicture.SourceFullName = MyDir + @"Images\Watermark.png";

            // Here we apply the PNG32.FLT filter
            fieldIncludePicture.GraphicFilter = "PNG32";
            fieldIncludePicture.IsLinked = true;
            fieldIncludePicture.ResizeHorizontally = true;
            fieldIncludePicture.ResizeVertically = true;

            // We can do the same thing with an IMPORT field
            FieldImport fieldImport = (FieldImport)builder.InsertField(FieldType.FieldImport, true);
            fieldImport.GraphicFilter = "PNG32";
            fieldImport.IsLinked = true;
            fieldImport.SourceFullName = MyDir + @"Images\Watermark.png";

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.IncludePicture.docx");
            //ExEnd
        }

        //ExStart
        //ExFor:FieldIncludeText
        //ExFor:FieldIncludeText.BookmarkName
        //ExFor:FieldIncludeText.Encoding
        //ExFor:FieldIncludeText.LockFields
        //ExFor:FieldIncludeText.MimeType
        //ExFor:FieldIncludeText.NamespaceMappings
        //ExFor:FieldIncludeText.SourceFullName
        //ExFor:FieldIncludeText.TextConverter
        //ExFor:FieldIncludeText.XPath
        //ExFor:FieldIncludeText.XslTransformation
        //ExSummary:Shows how to create an INCLUDETEXT field and set its properties.
        [Test] //ExSkip
        [Ignore("WORDSNET-17543")] //ExSkip
        public void FieldIncludeText()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Use a document builder to insert an include text field and perform an XSL transformation on an XML document
            FieldIncludeText fieldIncludeText = CreateFieldIncludeText(builder, MyDir + "Field.IncludeText.Source.xml", false, "text/xml", "XML", "ISO-8859-1");
            fieldIncludeText.XslTransformation = MyDir + "Field.IncludeText.Source.xsl";

            builder.Writeln();

            // Use a document builder to insert an include text field and use an XPath to take specific elements
            fieldIncludeText = CreateFieldIncludeText(builder, MyDir + "Field.IncludeText.Source.xml", false, "text/xml", "XML", "ISO-8859-1");
            fieldIncludeText.NamespaceMappings = "xmlns:n='myNamespace'";
            fieldIncludeText.XPath = "/catalog/cd/title";

            doc.Save(ArtifactsDir + "Field.IncludeText.docx");
        }

        /// <summary>
        /// Use a document builder to insert an INCLUDETEXT field and set its properties
        /// </summary>
        public FieldIncludeText CreateFieldIncludeText(DocumentBuilder builder, string sourceFullName, bool lockFields, string mimeType, string textConverter, string encoding)
        {
            FieldIncludeText fieldIncludeText = (FieldIncludeText)builder.InsertField(FieldType.FieldIncludeText, true);
            fieldIncludeText.SourceFullName = sourceFullName;
            fieldIncludeText.LockFields = lockFields;
            fieldIncludeText.MimeType = mimeType;
            fieldIncludeText.TextConverter = textConverter;
            fieldIncludeText.Encoding = encoding;

            return fieldIncludeText;
        }
        //ExEnd

        [Test]
        [Ignore("WORDSNET-17545")]
        public void FieldHyperlink()
        {
            //ExStart
            //ExFor:FieldHyperlink
            //ExFor:FieldHyperlink.Address
            //ExFor:FieldHyperlink.IsImageMap
            //ExFor:FieldHyperlink.OpenInNewWindow
            //ExFor:FieldHyperlink.ScreenTip
            //ExFor:FieldHyperlink.SubAddress
            //ExFor:FieldHyperlink.Target
            //ExSummary:Shows how to insert HYPERLINK fields.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a hyperlink with a document builder
            FieldHyperlink fieldHyperlink = (FieldHyperlink)builder.InsertField(FieldType.FieldHyperlink, true);

            // When link is clicked, open a document and place the cursor on the bookmarked location
            fieldHyperlink.Address = MyDir + "Field.HyperlinkDestination.docx";
            fieldHyperlink.SubAddress = "My_Bookmark";
            fieldHyperlink.ScreenTip = "Open " + fieldHyperlink.Address + " on bookmark " + fieldHyperlink.SubAddress + " in a new window";

            builder.Writeln();

            // Open html file at a specific frame
            fieldHyperlink = (FieldHyperlink)builder.InsertField(FieldType.FieldHyperlink, true);
            fieldHyperlink.Address = MyDir + "Field.HyperlinkDestination.html";
            fieldHyperlink.ScreenTip = "Open " + fieldHyperlink.Address;
            fieldHyperlink.Target = "iframe_3";
            fieldHyperlink.OpenInNewWindow = true;
            fieldHyperlink.IsImageMap = false;

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Hyperlink.docx");
            //ExEnd
        }

        //ExStart
        //ExFor:MergeFieldImageDimension
        //ExFor:MergeFieldImageDimension.#ctor
        //ExFor:MergeFieldImageDimension.#ctor(Double)
        //ExFor:MergeFieldImageDimension.#ctor(Double,MergeFieldImageDimensionUnit)
        //ExFor:MergeFieldImageDimension.Unit
        //ExFor:MergeFieldImageDimension.Value
        //ExFor:MergeFieldImageDimensionUnit
        //ExFor:ImageFieldMergingArgs
        //ExFor:ImageFieldMergingArgs.ImageFileName
        //ExFor:ImageFieldMergingArgs.ImageWidth
        //ExFor:ImageFieldMergingArgs.ImageHeight
        //ExSummary:Shows how to set the dimensions of merged images.
        [Test]
        public void MergeFieldImageDimension()
        {
            Document doc = new Document();

            // Insert a merge field where images will be placed during the mail merge
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertField("MERGEFIELD Image:ImageColumn");

            // Create a data table for the mail merge
            // The name of the column that contains our image filenames needs to match the name of our merge field
            System.Data.DataTable dataTable = CreateDataTable("Images", "ImageColumn",
                new string[]
                {
                    ImageDir + "Aspose.Words.gif",
                    ImageDir + "Watermark.png",
                    ImageDir + "dotnet-logo.png"
                });

            doc.MailMerge.FieldMergingCallback = new MergedImageResizer(450, 200, MergeFieldImageDimensionUnit.Point);
            doc.MailMerge.Execute(dataTable);

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.MergeFieldImageDimension.docx");
        }

        /// <summary>
        /// Creates a data table with a single column
        /// </summary>
        private System.Data.DataTable CreateDataTable(string tableName, string columnName, string[] columnContents)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable(tableName);
            dataTable.Columns.Add(new System.Data.DataColumn(columnName));

            foreach (string s in columnContents)
            {
                System.Data.DataRow dataRow = dataTable.NewRow();
                dataRow[0] = s;
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        /// <summary>
        /// Sets the size of all mail merged images to one defined width and height 
        /// </summary>
        private class MergedImageResizer : IFieldMergingCallback
        {
            public MergedImageResizer(double imageWidth, double imageHeight, MergeFieldImageDimensionUnit unit)
            {
                mImageWidth = imageWidth;
                mImageHeight = imageHeight;
                mUnit = unit;
            }

            public void FieldMerging(FieldMergingArgs e)
            {
                throw new NotImplementedException();
            }

            public void ImageFieldMerging(ImageFieldMergingArgs args)
            {
                args.ImageFileName = args.FieldValue.ToString();
                args.ImageWidth = new MergeFieldImageDimension(mImageWidth, mUnit);
                args.ImageHeight = new MergeFieldImageDimension(mImageHeight, mUnit);

                Assert.AreEqual(mImageWidth, args.ImageWidth.Value);
                Assert.AreEqual(mUnit, args.ImageWidth.Unit);
                Assert.AreEqual(mImageHeight, args.ImageHeight.Value);
                Assert.AreEqual(mUnit, args.ImageHeight.Unit);
            }

            private readonly double mImageWidth;
            private readonly double mImageHeight;
            private readonly MergeFieldImageDimensionUnit mUnit;
        }
        //ExEnd

        [Test]
        [Ignore("WORDSNET-17524")]
        public void FieldXE()
        {
            //ExStart
            //ExFor:FieldIndex
            //ExFor:FieldIndex.BookmarkName
            //ExFor:FieldIndex.CrossReferenceSeparator
            //ExFor:FieldIndex.EntryType
            //ExFor:FieldIndex.HasPageNumberSeparator
            //ExFor:FieldIndex.HasSequenceName
            //ExFor:FieldIndex.Heading
            //ExFor:FieldIndex.LanguageId
            //ExFor:FieldIndex.LetterRange
            //ExFor:FieldIndex.NumberOfColumns
            //ExFor:FieldIndex.PageNumberListSeparator
            //ExFor:FieldIndex.PageNumberSeparator
            //ExFor:FieldIndex.PageRangeSeparator
            //ExFor:FieldIndex.RunSubentriesOnSameLine
            //ExFor:FieldIndex.SequenceName
            //ExFor:FieldIndex.SequenceSeparator
            //ExFor:FieldIndex.UseYomi
            //ExFor:FieldXE
            //ExFor:FieldXE.EntryType
            //ExFor:FieldXE.HasPageRangeBookmarkName
            //ExFor:FieldXE.IsBold
            //ExFor:FieldXE.IsItalic
            //ExFor:FieldXE.PageNumberReplacement
            //ExFor:FieldXE.PageRangeBookmarkName
            //ExFor:FieldXE.Text
            //ExFor:FieldXE.Yomi
            //ExSummary:Shows how to populate an index field with index entries.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create an index field which will contain all the index entries
            FieldIndex index = (FieldIndex)builder.InsertField(FieldType.FieldIndex, true);

            // Bookmark that will encompass a section that we want to index
            string mainBookmarkName = "MainBookmark";
            builder.StartBookmark(mainBookmarkName);
            index.BookmarkName = mainBookmarkName;
            index.CrossReferenceSeparator = ":";
            index.Heading = ">";
            index.LanguageId = "1033";
            index.LetterRange = "a-j";
            index.NumberOfColumns = "2";
            index.PageNumberListSeparator = "|";
            index.PageNumberSeparator = "|";
            index.PageRangeSeparator = "/";
            index.UseYomi = true;
            index.RunSubentriesOnSameLine = false;
            index.SequenceName = "Chapter";
            index.SequenceSeparator = ":";
            Assert.IsTrue(index.HasPageNumberSeparator);
            Assert.IsTrue(index.HasSequenceName);

            // Our index will take up page 1
            builder.InsertBreak(BreakType.PageBreak);

            // Use a document builder to insert an index entry
            // Index entries are not added to the index manually, it will find them on its own
            FieldXE indexEntry = (FieldXE)builder.InsertField(FieldType.FieldIndexEntry, true);
            indexEntry.Text = "Index entry 1";
            indexEntry.EntryType = "Type1";
            indexEntry.IsBold = true;
            indexEntry.IsItalic = true;
            Assert.AreEqual(false, indexEntry.HasPageRangeBookmarkName);

            // We can insert a bookmark and have the index field point to it
            string subBookmarkName = "MyBookmark";
            builder.StartBookmark(subBookmarkName);
            builder.Writeln("Bookmark text contents.");
            builder.EndBookmark(subBookmarkName);

            // Put the bookmark and index entry field on different pages
            // Our index will use the page that the bookmark is on, not that of the index entry field, as the page number
            builder.InsertBreak(BreakType.PageBreak);
            indexEntry = (FieldXE)builder.InsertField(FieldType.FieldIndexEntry, true);
            indexEntry.Text = "Index entry 2";
            indexEntry.EntryType = "Type1";
            indexEntry.PageRangeBookmarkName = subBookmarkName;
            Assert.AreEqual(true, indexEntry.HasPageRangeBookmarkName);

            // We can use the PageNumberReplacement property to point to any page we want, even one that may not exist
            builder.InsertBreak(BreakType.PageBreak);
            indexEntry = (FieldXE)builder.InsertField(FieldType.FieldIndexEntry, true);
            indexEntry.Text = "Index entry 3";
            indexEntry.EntryType = "Type1";
            indexEntry.PageNumberReplacement = "999";

            // If we are using an East Asian language, we can sort entries phonetically (using Furigana) instead of alphabetically
            indexEntry = (FieldXE)builder.InsertField(FieldType.FieldIndexEntry, true);
            indexEntry.Text = "漢字";
            indexEntry.EntryType = "Type1";

            // The Yomi field will contain the character looked up for sorting
            indexEntry.Yomi = "か";

            // If we are sorting phonetically, we need to notify the index
            index.UseYomi = true;

            // For all our entry fields, we set the entry type to "Type1"
            // Our field index will not list those entries unless we set its entry type to that of the entries
            index.EntryType = "Type1";

            builder.EndBookmark(mainBookmarkName);

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.XE.docx");
            //ExEnd
        }

        [Test]
        public void FieldBarcode()
        {
            //ExStart
            //ExFor:FieldBarcode
            //ExFor:FieldBarcode.FacingIdentificationMark
            //ExFor:FieldBarcode.IsBookmark
            //ExFor:FieldBarcode.IsUSPostalAddress
            //ExFor:FieldBarcode.PostalAddress
            //ExSummary:Shows how to insert a BARCODE field and set its properties. 
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Use a document builder to insert a bookmark with a US postal code in it
            builder.StartBookmark("BarcodeBookmark");
            builder.Writeln("96801");
            builder.EndBookmark("BarcodeBookmark");

            builder.Writeln();

            // Reference a US postal code directly
            FieldBarcode fieldBarcode = (FieldBarcode)builder.InsertField(FieldType.FieldBarcode, true);
            fieldBarcode.FacingIdentificationMark = "C";
            fieldBarcode.PostalAddress = "96801";
            fieldBarcode.IsUSPostalAddress = true;

            builder.Writeln();

            // Reference a US postal code from a bookmark
            fieldBarcode = (FieldBarcode)builder.InsertField(FieldType.FieldBarcode, true);
            fieldBarcode.PostalAddress = "BarcodeBookmark";
            fieldBarcode.IsBookmark = true;

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.USAddressBarcode.docx");
            //ExEnd
        }

        [Test]
        public void FieldDisplayBarcode()
        {
            //ExStart
            //ExFor:FieldDisplayBarcode
            //ExFor:FieldDisplayBarcode.AddStartStopChar
            //ExFor:FieldDisplayBarcode.BackgroundColor
            //ExFor:FieldDisplayBarcode.BarcodeType
            //ExFor:FieldDisplayBarcode.BarcodeValue
            //ExFor:FieldDisplayBarcode.CaseCodeStyle
            //ExFor:FieldDisplayBarcode.DisplayText
            //ExFor:FieldDisplayBarcode.ErrorCorrectionLevel
            //ExFor:FieldDisplayBarcode.FixCheckDigit
            //ExFor:FieldDisplayBarcode.ForegroundColor
            //ExFor:FieldDisplayBarcode.PosCodeStyle
            //ExFor:FieldDisplayBarcode.ScalingFactor
            //ExFor:FieldDisplayBarcode.SymbolHeight
            //ExFor:FieldDisplayBarcode.SymbolRotation
            //ExSummary:Shows how to insert a DISPLAYBARCODE field and set its properties. 
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            FieldDisplayBarcode field = (FieldDisplayBarcode)builder.InsertField(FieldType.FieldDisplayBarcode, true);

            // Insert a QR code
            field.BarcodeType = "QR";
            field.BarcodeValue = "ABC123";
            field.BackgroundColor = "0xF8BD69";
            field.ForegroundColor = "0xB5413B";
            field.ErrorCorrectionLevel = "3";
            field.ScalingFactor = "250";
            field.SymbolHeight = "1000";
            field.SymbolRotation = "0";

            Assert.AreEqual(" DISPLAYBARCODE  ABC123 QR \\b 0xF8BD69 \\f 0xB5413B \\q 3 \\s 250 \\h 1000 \\r 0", field.GetFieldCode());
            builder.Writeln();

            // insert a EAN13 barcode
            field = (FieldDisplayBarcode)builder.InsertField(FieldType.FieldDisplayBarcode, true);
            field.BarcodeType = "EAN13";
            field.BarcodeValue = "501234567890";
            field.DisplayText = true;
            field.PosCodeStyle = "CASE";
            field.FixCheckDigit = true;

            Assert.AreEqual(" DISPLAYBARCODE  501234567890 EAN13 \\t \\p CASE \\x", field.GetFieldCode());
            builder.Writeln();

            // insert a CODE39 barcode
            field = (FieldDisplayBarcode)builder.InsertField(FieldType.FieldDisplayBarcode, true);
            field.BarcodeType = "CODE39";
            field.BarcodeValue = "12345ABCDE";
            field.AddStartStopChar = true;

            Assert.AreEqual(" DISPLAYBARCODE  12345ABCDE CODE39 \\d", field.GetFieldCode());
            builder.Writeln();

            // insert a ITF14 barcode
            field = (FieldDisplayBarcode)builder.InsertField(FieldType.FieldDisplayBarcode, true);
            field.BarcodeType = "ITF14";
            field.BarcodeValue = "09312345678907";
            field.CaseCodeStyle = "STD";

            Assert.AreEqual(" DISPLAYBARCODE  09312345678907 ITF14 \\c STD", field.GetFieldCode());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.DisplayBarcode.docx");
            //ExEnd
        }

        //ExStart
        //ExFor:FieldMergeBarcode
        //ExFor:FieldMergeBarcode.AddStartStopChar
        //ExFor:FieldMergeBarcode.BackgroundColor
        //ExFor:FieldMergeBarcode.BarcodeType
        //ExFor:FieldMergeBarcode.BarcodeValue
        //ExFor:FieldMergeBarcode.CaseCodeStyle
        //ExFor:FieldMergeBarcode.DisplayText
        //ExFor:FieldMergeBarcode.ErrorCorrectionLevel
        //ExFor:FieldMergeBarcode.FixCheckDigit
        //ExFor:FieldMergeBarcode.ForegroundColor
        //ExFor:FieldMergeBarcode.PosCodeStyle
        //ExFor:FieldMergeBarcode.ScalingFactor
        //ExFor:FieldMergeBarcode.SymbolHeight
        //ExFor:FieldMergeBarcode.SymbolRotation
        //ExSummary:Shows how to use MERGEBARCODE fields to integrate barcodes into mail merge operations.
        [Test] //ExSkip
        public void FieldMergeBarcode_QR()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a QR code
            FieldMergeBarcode field = (FieldMergeBarcode) builder.InsertField(FieldType.FieldMergeBarcode, true);
            field.BarcodeType = "QR";

            // In a DISPLAYBARCODE field, the BarcodeValue attribute decides what value the barcode will display
            // However in our MERGEBARCODE fields, it has the same function as the FieldName attribute of a MERGEFIELD
            field.BarcodeValue = "MyQRCode";
            field.BackgroundColor = "0xF8BD69";
            field.ForegroundColor = "0xB5413B";
            field.ErrorCorrectionLevel = "3";
            field.ScalingFactor = "250";
            field.SymbolHeight = "1000";
            field.SymbolRotation = "0";

            Assert.AreEqual(" MERGEBARCODE  MyQRCode QR \\b 0xF8BD69 \\f 0xB5413B \\q 3 \\s 250 \\h 1000 \\r 0",
                field.GetFieldCode());
            builder.Writeln();

            // Create a data source for our mail merge
            // This source is a data table, whose column names correspond to the FieldName attributes of MERGEFIELD fields
            // as well as BarcodeValue attributes of DISPLAYBARCODE fields
            DataTable table = CreateTable("Barcodes", new[] { "MyQRCode" },
                new[,] { { "ABC123" }, { "DEF456" } });

            // During the mail merge, all our MERGEBARCODE fields will be converted into DISPLAYBARCODE fields,
            // with values from the data table rows deposited into corresponding BarcodeValue attributes
            doc.MailMerge.Execute(table);

            Assert.AreEqual(FieldType.FieldDisplayBarcode, doc.Range.Fields[0].Type);
            Assert.AreEqual(FieldType.FieldDisplayBarcode, doc.Range.Fields[1].Type);

            Assert.AreEqual("DISPLAYBARCODE \"ABC123\" QR \\q 3 \\s 250 \\h 1000 \\r 0 \\b 0xF8BD69 \\f 0xB5413B",
                doc.Range.Fields[0].GetFieldCode());
            Assert.AreEqual("DISPLAYBARCODE \"DEF456\" QR \\q 3 \\s 250 \\h 1000 \\r 0 \\b 0xF8BD69 \\f 0xB5413B",
                doc.Range.Fields[1].GetFieldCode());

            doc.Save(ArtifactsDir + "Field.MergeBarcode_QR.docx");
        }

        [Test] //ExSkip
        public void FieldMergeBarcode_EAN13()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a EAN13 barcode
            FieldMergeBarcode field = (FieldMergeBarcode) builder.InsertField(FieldType.FieldMergeBarcode, true);
            field.BarcodeType = "EAN13";
            field.BarcodeValue = "MyEAN13Barcode";
            field.DisplayText = true;
            field.PosCodeStyle = "CASE";
            field.FixCheckDigit = true;

            Assert.AreEqual(" MERGEBARCODE  MyEAN13Barcode EAN13 \\t \\p CASE \\x", field.GetFieldCode());
            builder.Writeln();

            DataTable table = CreateTable("Barcodes", new[] { "MyEAN13Barcode" },
                new[,] { { "501234567890" }, { "123456789012" } });

            doc.MailMerge.Execute(table);

            Assert.AreEqual(FieldType.FieldDisplayBarcode, doc.Range.Fields[0].Type);
            Assert.AreEqual(FieldType.FieldDisplayBarcode, doc.Range.Fields[1].Type);

            Assert.AreEqual("DISPLAYBARCODE \"501234567890\" EAN13 \\t \\p CASE \\x",
                doc.Range.Fields[0].GetFieldCode());
            Assert.AreEqual("DISPLAYBARCODE \"123456789012\" EAN13 \\t \\p CASE \\x",
                doc.Range.Fields[1].GetFieldCode());

            doc.Save(ArtifactsDir + "Field.MergeBarcode_EAN13.docx");
        }

        [Test] //ExSkip
        public void FieldMergeBarcode_CODE39()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a CODE39 barcode
            FieldMergeBarcode field = (FieldMergeBarcode) builder.InsertField(FieldType.FieldMergeBarcode, true);
            field.BarcodeType = "CODE39";
            field.BarcodeValue = "MyCODE39Barcode";
            field.AddStartStopChar = true;

            Assert.AreEqual(" MERGEBARCODE  MyCODE39Barcode CODE39 \\d", field.GetFieldCode());
            builder.Writeln();

            DataTable table = CreateTable("Barcodes", new[] { "MyCODE39Barcode" },
                new[,] { { "12345ABCDE" }, { "67890FGHIJ" } });

            doc.MailMerge.Execute(table);

            Assert.AreEqual(FieldType.FieldDisplayBarcode, doc.Range.Fields[0].Type);
            Assert.AreEqual(FieldType.FieldDisplayBarcode, doc.Range.Fields[1].Type);

            Assert.AreEqual("DISPLAYBARCODE \"12345ABCDE\" CODE39 \\d",
                doc.Range.Fields[0].GetFieldCode());
            Assert.AreEqual("DISPLAYBARCODE \"67890FGHIJ\" CODE39 \\d",
                doc.Range.Fields[1].GetFieldCode());

            doc.Save(ArtifactsDir + "Field.MergeBarcode_CODE39.docx");
        }

        [Test] //ExSkip
        public void FieldMergeBarcode_ITF14()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a ITF14 barcode
            FieldMergeBarcode field = (FieldMergeBarcode) builder.InsertField(FieldType.FieldMergeBarcode, true);
            field.BarcodeType = "ITF14";
            field.BarcodeValue = "MyITF14Barcode";
            field.CaseCodeStyle = "STD";

            Assert.AreEqual(" MERGEBARCODE  MyITF14Barcode ITF14 \\c STD", field.GetFieldCode());

            DataTable table = CreateTable("Barcodes", new[] { "MyITF14Barcode" },
                new[,] { { "09312345678907" }, { "1234567891234" } });

            doc.MailMerge.Execute(table);

            Assert.AreEqual(FieldType.FieldDisplayBarcode, doc.Range.Fields[0].Type);
            Assert.AreEqual(FieldType.FieldDisplayBarcode, doc.Range.Fields[1].Type);

            Assert.AreEqual("DISPLAYBARCODE \"09312345678907\" ITF14 \\c STD",
                doc.Range.Fields[0].GetFieldCode());
            Assert.AreEqual("DISPLAYBARCODE \"1234567891234\" ITF14 \\c STD",
                doc.Range.Fields[1].GetFieldCode());

            doc.Save(ArtifactsDir + "Field.MergeBarcode_ITF14.docx");
        }

        /// <summary>
        /// Creates a DataTable named by dataTableName, adds a column for every element in columnNames
        /// and fills rows with data from dataSet
        /// </summary>
        public DataTable CreateTable(string dataTableName, string[] columnNames, object[,] dataSet)
        {
            if (dataTableName != string.Empty || columnNames.Length != 0)
            {
                DataTable table = new DataTable(dataTableName);

                foreach (string columnName in columnNames)
                {
                    table.Columns.Add(columnName);
                }

                foreach (object data in dataSet)
                {
                    table.Rows.Add(data);
                }

                return table;
            }

            throw new ArgumentException("DataTable name and Column name must be declared.");
        }
        //ExEnd

#if !(NETSTANDARD2_0 || __MOBILE__)
        //ExStart
        //ExFor:BarcodeParameters
        //ExFor:BarcodeParameters.AddStartStopChar
        //ExFor:BarcodeParameters.BackgroundColor
        //ExFor:BarcodeParameters.BarcodeType
        //ExFor:BarcodeParameters.BarcodeValue
        //ExFor:BarcodeParameters.CaseCodeStyle
        //ExFor:BarcodeParameters.DisplayText
        //ExFor:BarcodeParameters.ErrorCorrectionLevel
        //ExFor:BarcodeParameters.FacingIdentificationMark
        //ExFor:BarcodeParameters.FixCheckDigit
        //ExFor:BarcodeParameters.ForegroundColor
        //ExFor:BarcodeParameters.IsBookmark
        //ExFor:BarcodeParameters.IsUSPostalAddress
        //ExFor:BarcodeParameters.PosCodeStyle
        //ExFor:BarcodeParameters.PostalAddress
        //ExFor:BarcodeParameters.ScalingFactor
        //ExFor:BarcodeParameters.SymbolHeight
        //ExFor:BarcodeParameters.SymbolRotation
        //ExFor:IBarcodeGenerator
        //ExFor:IBarcodeGenerator.GetBarcodeImage(BarcodeParameters)
        //ExFor:IBarcodeGenerator.GetOldBarcodeImage(BarcodeParameters)
        //ExFor:FieldOptions.BarcodeGenerator
        //ExSummary:Shows how to create barcode images using a barcode generator.
        [Test] //ExSkip
        public void BarcodeGenerator()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Assert.IsNull(doc.FieldOptions.BarcodeGenerator);

            // Barcodes generated in this way will be images, and we can use a custom IBarcodeGenerator implementation to generate them
            doc.FieldOptions.BarcodeGenerator = new CustomBarcodeGenerator();

            // Configure barcode parameters for a QR barcode
            BarcodeParameters barcodeParameters = new BarcodeParameters();
            barcodeParameters.BarcodeType = "QR";
            barcodeParameters.BarcodeValue = "ABC123";
            barcodeParameters.BackgroundColor = "0xF8BD69";
            barcodeParameters.ForegroundColor = "0xB5413B";
            barcodeParameters.ErrorCorrectionLevel = "3";
            barcodeParameters.ScalingFactor = "250";
            barcodeParameters.SymbolHeight = "1000";
            barcodeParameters.SymbolRotation = "0";

            // Save the generated barcode image to the file system
            System.Drawing.Image img = doc.FieldOptions.BarcodeGenerator.GetBarcodeImage(barcodeParameters);
            img.Save(ArtifactsDir + "Field.BarcodeGenerator.QR.jpg");

            // Insert the image into the document
            builder.InsertImage(img);

            // Configure barcode parameters for a EAN13 barcode
            barcodeParameters = new BarcodeParameters();
            barcodeParameters.BarcodeType = "EAN13";
            barcodeParameters.BarcodeValue = "501234567890";
            barcodeParameters.DisplayText = true;
            barcodeParameters.PosCodeStyle = "CASE";
            barcodeParameters.FixCheckDigit = true;

            img = doc.FieldOptions.BarcodeGenerator.GetBarcodeImage(barcodeParameters);
            img.Save(ArtifactsDir + "Field.BarcodeGenerator.EAN13.jpg");
            builder.InsertImage(img);

            // Configure barcode parameters for a CODE39 barcode
            barcodeParameters = new BarcodeParameters();
            barcodeParameters.BarcodeType = "CODE39";
            barcodeParameters.BarcodeValue = "12345ABCDE";
            barcodeParameters.AddStartStopChar = true;

            img = doc.FieldOptions.BarcodeGenerator.GetBarcodeImage(barcodeParameters);
            img.Save(ArtifactsDir + "Field.BarcodeGenerator.CODE39.jpg");
            builder.InsertImage(img);

            // Configure barcode parameters for an ITF14 barcode
            barcodeParameters = new BarcodeParameters();
            barcodeParameters.BarcodeType = "ITF14";
            barcodeParameters.BarcodeValue = "09312345678907";
            barcodeParameters.CaseCodeStyle = "STD";

            img = doc.FieldOptions.BarcodeGenerator.GetBarcodeImage(barcodeParameters);
            img.Save(ArtifactsDir + "Field.BarcodeGenerator.ITF14.jpg");
            builder.InsertImage(img);

            doc.Save(ArtifactsDir + "Field.BarcodeGenerator.docx");
        }
        //ExEnd
#endif

        //ExStart
        //ExFor:FieldLink
        //ExFor:FieldLink.AutoUpdate
        //ExFor:FieldLink.FormatUpdateType
        //ExFor:FieldLink.InsertAsBitmap
        //ExFor:FieldLink.InsertAsHtml
        //ExFor:FieldLink.InsertAsPicture
        //ExFor:FieldLink.InsertAsRtf
        //ExFor:FieldLink.InsertAsText
        //ExFor:FieldLink.InsertAsUnicode
        //ExFor:FieldLink.IsLinked
        //ExFor:FieldLink.ProgId
        //ExFor:FieldLink.SourceFullName
        //ExFor:FieldLink.SourceItem
        //ExFor:FieldDde
        //ExFor:FieldDde.AutoUpdate
        //ExFor:FieldDde.InsertAsBitmap
        //ExFor:FieldDde.InsertAsHtml
        //ExFor:FieldDde.InsertAsPicture
        //ExFor:FieldDde.InsertAsRtf
        //ExFor:FieldDde.InsertAsText
        //ExFor:FieldDde.InsertAsUnicode
        //ExFor:FieldDde.IsLinked
        //ExFor:FieldDde.ProgId
        //ExFor:FieldDde.SourceFullName
        //ExFor:FieldDde.SourceItem
        //ExFor:FieldDdeAuto
        //ExFor:FieldDdeAuto.InsertAsBitmap
        //ExFor:FieldDdeAuto.InsertAsHtml
        //ExFor:FieldDdeAuto.InsertAsPicture
        //ExFor:FieldDdeAuto.InsertAsRtf
        //ExFor:FieldDdeAuto.InsertAsText
        //ExFor:FieldDdeAuto.InsertAsUnicode
        //ExFor:FieldDdeAuto.IsLinked
        //ExFor:FieldDdeAuto.ProgId
        //ExFor:FieldDdeAuto.SourceFullName
        //ExFor:FieldDdeAuto.SourceItem
        //ExSummary:Shows how to insert linked objects as LINK, DDE and DDEAUTO fields and present them within the document in different ways.
        [Test] //ExSkip
        [TestCase(InsertLinkedObjectAs.Text)] //ExSkip
        [TestCase(InsertLinkedObjectAs.Unicode)] //ExSkip
        [TestCase(InsertLinkedObjectAs.Html)] //ExSkip
        [TestCase(InsertLinkedObjectAs.Rtf)] //ExSkip
        [Ignore("WORDSNET-16226")] //ExSkip
        public void FieldLinkedObjectsAsText(InsertLinkedObjectAs insertLinkedObjectAs)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert fields containing text from another document and present them as text (see InsertLinkedObjectAs enum).
            builder.Writeln("FieldLink:\n");
            InsertFieldLink(builder, insertLinkedObjectAs, "Word.Document.8", MyDir + "Document.doc", null, true);

            builder.Writeln("FieldDde:\n");
            InsertFieldDde(builder, insertLinkedObjectAs, "Excel.Sheet", MyDir + "Document.Spreadsheet.xlsx",
                "Sheet1!R1C1", true, true);

            builder.Writeln("FieldDdeAuto:\n");
            InsertFieldDdeAuto(builder, insertLinkedObjectAs, "Excel.Sheet", MyDir + "Document.Spreadsheet.xlsx",
                "Sheet1!R1C1", true);

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.LinkedObjectsAsText.docx");
        }

        [Test] //ExSkip
        [TestCase(InsertLinkedObjectAs.Picture)] //ExSkip
        [TestCase(InsertLinkedObjectAs.Bitmap)] //ExSkip
        [Ignore("WORDSNET-16226")] //ExSkip
        public void FieldLinkedObjectsAsImage(InsertLinkedObjectAs insertLinkedObjectAs)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert one cell from a spreadsheet as an image (see InsertLinkedObjectAs enum).
            builder.Writeln("FieldLink:\n");
            InsertFieldLink(builder, insertLinkedObjectAs, "Excel.Sheet", MyDir + "MySpreadsheet.xlsx",
                "Sheet1!R2C2", true);

            builder.Writeln("FieldDde:\n");
            InsertFieldDde(builder, insertLinkedObjectAs, "Excel.Sheet", MyDir + "Document.Spreadsheet.xlsx",
                "Sheet1!R1C1", true, true);

            builder.Writeln("FieldDdeAuto:\n");
            InsertFieldDdeAuto(builder, insertLinkedObjectAs, "Excel.Sheet", MyDir + "Document.Spreadsheet.xlsx",
                "Sheet1!R1C1", true);

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.LinkedObjectsAsImage.docx");
        }

        /// <summary>
        /// Use a document builder to insert a LINK field and set its properties according to parameters
        /// </summary>
        private void InsertFieldLink(DocumentBuilder builder, InsertLinkedObjectAs insertLinkedObjectAs,
            string progId, string sourceFullName, string sourceItem, bool shouldAutoUpdate)
        {
            FieldLink field = (FieldLink)builder.InsertField(FieldType.FieldLink, true);

            switch (insertLinkedObjectAs)
            {
                case InsertLinkedObjectAs.Text:
                    field.InsertAsText = true;
                    break;
                case InsertLinkedObjectAs.Unicode:
                    field.InsertAsUnicode = true;
                    break;
                case InsertLinkedObjectAs.Html:
                    field.InsertAsHtml = true;
                    break;
                case InsertLinkedObjectAs.Rtf:
                    field.InsertAsRtf = true;
                    break;
                case InsertLinkedObjectAs.Picture:
                    field.InsertAsPicture = true;
                    break;
                case InsertLinkedObjectAs.Bitmap:
                    field.InsertAsBitmap = true;
                    break;
            }

            field.AutoUpdate = shouldAutoUpdate;
            field.ProgId = progId;
            field.SourceFullName = sourceFullName;
            field.SourceItem = sourceItem;

            builder.Writeln("\n");
        }

        /// <summary>
        /// Use a document builder to insert a DDE field and set its properties according to parameters
        /// </summary>
        private void InsertFieldDde(DocumentBuilder builder, InsertLinkedObjectAs insertLinkedObjectAs, string progId,
            string sourceFullName, string sourceItem, bool isLinked, bool shouldAutoUpdate)
        {
            FieldDde field = (FieldDde)builder.InsertField(FieldType.FieldDDE, true);

            switch (insertLinkedObjectAs)
            {
                case InsertLinkedObjectAs.Text:
                    field.InsertAsText = true;
                    break;
                case InsertLinkedObjectAs.Unicode:
                    field.InsertAsUnicode = true;
                    break;
                case InsertLinkedObjectAs.Html:
                    field.InsertAsHtml = true;
                    break;
                case InsertLinkedObjectAs.Rtf:
                    field.InsertAsRtf = true;
                    break;
                case InsertLinkedObjectAs.Picture:
                    field.InsertAsPicture = true;
                    break;
                case InsertLinkedObjectAs.Bitmap:
                    field.InsertAsBitmap = true;
                    break;
            }

            field.AutoUpdate = shouldAutoUpdate;
            field.ProgId = progId;
            field.SourceFullName = sourceFullName;
            field.SourceItem = sourceItem;
            field.IsLinked = isLinked;

            builder.Writeln("\n");
        }

        /// <summary>
        /// Use a document builder to insert a DDEAUTO field and set its properties according to parameters
        /// </summary>
        private void InsertFieldDdeAuto(DocumentBuilder builder, InsertLinkedObjectAs insertLinkedObjectAs,
            string progId, string sourceFullName, string sourceItem, bool isLinked)
        {
            FieldDdeAuto field = (FieldDdeAuto)builder.InsertField(FieldType.FieldDDEAuto, true);

            switch (insertLinkedObjectAs)
            {
                case InsertLinkedObjectAs.Text:
                    field.InsertAsText = true;
                    break;
                case InsertLinkedObjectAs.Unicode:
                    field.InsertAsUnicode = true;
                    break;
                case InsertLinkedObjectAs.Html:
                    field.InsertAsHtml = true;
                    break;
                case InsertLinkedObjectAs.Rtf:
                    field.InsertAsRtf = true;
                    break;
                case InsertLinkedObjectAs.Picture:
                    field.InsertAsPicture = true;
                    break;
                case InsertLinkedObjectAs.Bitmap:
                    field.InsertAsBitmap = true;
                    break;
            }

            field.ProgId = progId;
            field.SourceFullName = sourceFullName;
            field.SourceItem = sourceItem;
            field.IsLinked = isLinked;
        }

        public enum InsertLinkedObjectAs
        {
            // LinkedObjectAsText
            Text,
            Unicode,
            Html,
            Rtf,
            // LinkedObjectAsImage
            Picture,
            Bitmap
        }
        //ExEnd

        [Test]
        public void FieldOptionsCurrentUser()
        {
            //ExStart
            //ExFor:FieldOptions.CurrentUser
            //ExFor:FieldOptions.DefaultDocumentAuthor
            //ExFor:UserInformation
            //ExFor:UserInformation.Name
            //ExFor:UserInformation.Initials
            //ExFor:UserInformation.Address
            //ExSummary:Shows how to set user details and insert them as fields.
            Document doc = new Document();

            // Set user information
            UserInformation userInformation = new UserInformation();
            userInformation.Name = "John Doe";
            userInformation.Initials = "J. D.";
            userInformation.Address = "123 Main Street";
            doc.FieldOptions.CurrentUser = userInformation;

            // Insert fields that reference our user information
            DocumentBuilder builder = new DocumentBuilder(doc);
            Assert.AreEqual(userInformation.Name, builder.InsertField(" USERNAME ").Result);
            Assert.AreEqual(userInformation.Initials, builder.InsertField(" USERINITIALS ").Result);
            Assert.AreEqual(userInformation.Address, builder.InsertField(" USERADDRESS ").Result);
            //ExEnd
        }

        [Test]
        public void FieldOptionsFileName()
        {
            //ExStart
            //ExFor:FieldOptions.FileName
            //ExFor:FieldFileName
            //ExFor:FieldFileName.IncludeFullPath
            //ExSummary:Shows how to use FieldOptions to override the default value for the FILENAME field.
            Document doc = new Document(MyDir + "Document.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToDocumentEnd();
            builder.Writeln();

            // This FILENAME field will display the file name of the document we opened
            FieldFileName field = (FieldFileName)builder.InsertField(FieldType.FieldFileName, true);
            field.Update();

            Assert.AreEqual(" FILENAME ", field.GetFieldCode());
            Assert.AreEqual("Document.docx", field.Result);

            builder.Writeln();

            // By default, the FILENAME field does not show the full path, and we can change this
            field = (FieldFileName)builder.InsertField(FieldType.FieldFileName, true);
            field.IncludeFullPath = true;

            // We can override the values displayed by our FILENAME fields by setting this attribute
            Assert.IsNull(doc.FieldOptions.FileName);
            doc.FieldOptions.FileName = "Field.FileName.docx";
            field.Update();

            Assert.AreEqual(" FILENAME  \\p", field.GetFieldCode());
            Assert.AreEqual("Field.FileName.docx", field.Result);

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "" + doc.FieldOptions.FileName);
            //ExEnd
        }

        [Test]
        public void FieldOptionsBidi()
        {
            //ExStart
            //ExFor:FieldOptions.IsBidiTextSupportedOnUpdate
            //ExSummary:Shows how to use FieldOptions to ensure that bi-directional text is properly supported during the field update.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Ensure that any field operation involving right-to-left text is performed correctly 
            doc.FieldOptions.IsBidiTextSupportedOnUpdate = true;

            // Use a document builder to insert a field which contains right-to-left text
            FormField comboBox = builder.InsertComboBox("MyComboBox", new[] { "עֶשְׂרִים", "שְׁלוֹשִׁים", "אַרְבָּעִים", "חֲמִשִּׁים", "שִׁשִּׁים" }, 0);
            comboBox.CalculateOnExit = true;

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FieldOptionsBidi.docx");
            //ExEnd
        }

        [Test]
        public void FieldOptionsLegacyNumberFormat()
        {
            //ExStart
            //ExFor:FieldOptions.LegacyNumberFormat
            //ExSummary:Shows how use FieldOptions to change the number format.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Field field = builder.InsertField("= 2 + 3 \\# $##");

            Assert.AreEqual("$ 5", field.Result);

            doc.FieldOptions.LegacyNumberFormat = true;
            field.Update();

            Assert.AreEqual("$5", field.Result);
            //ExEnd
        }

        [Test]
        public void FieldOptionsPreProcessCulture()
        {
            //ExStart
            //ExFor:FieldOptions.PreProcessCulture
            //ExSummary:Shows how to set the preprocess culture.
            Document doc = new Document(MyDir + "Document.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            doc.FieldOptions.PreProcessCulture = new CultureInfo("de-DE");

            Field field = builder.InsertField(" DOCPROPERTY CreateTime");

            // Conforming to the German culture, the date/time will be presented in the "dd.mm.yyyy hh:mm" format
            Assert.IsTrue(Regex.Match(field.Result, @"\d{2}[.]\d{2}[.]\d{4} \d{2}[:]\d{2}").Success);

            doc.FieldOptions.PreProcessCulture = CultureInfo.InvariantCulture;
            field.Update();

            // After switching to the invariant culture, the date/time will be presented in the "mm/dd/yyyy hh:mm" format
            Assert.IsTrue(Regex.Match(field.Result, @"\d{2}[/]\d{2}[/]\d{4} \d{2}[:]\d{2}").Success);
            //ExEnd
        }

        [Test]
        public void FieldOptionsToaCategories()
        {
            //ExStart
            //ExFor:FieldOptions.ToaCategories
            //ExFor:ToaCategories
            //ExFor:ToaCategories.Item(Int32)
            //ExSummary:Shows how to specify a table of authorities categories for a document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // There are default category values we can use, or we can make our own like this
            ToaCategories toaCategories = new ToaCategories();
            doc.FieldOptions.ToaCategories = toaCategories;
            
            toaCategories[1] = "My Category 1"; // Replaces default value "Cases"
            toaCategories[2] = "My Category 2"; // Replaces default value "Statutes"

            // Insert 2 tables of authorities, one per category
            builder.InsertField("TOA \\c 1 \\h", null);
            builder.InsertField("TOA \\c 2 \\h", null);
            builder.InsertBreak(BreakType.PageBreak);

            // Insert table of authorities entries across 2 categories
            builder.InsertField("TA \\c 2 \\l \"entry 1\"");
            builder.InsertBreak(BreakType.PageBreak);
            builder.InsertField("TA \\c 1 \\l \"entry 2\"");
            builder.InsertBreak(BreakType.PageBreak);
            builder.InsertField("TA \\c 2 \\l \"entry 3\"");

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.TableOfAuthorities.Categories.docx");
            //ExEnd
        }

        [Test]
        public void FieldOptionsUseInvariantCultureNumberFormat()
        {
            //ExStart
            //ExFor:FieldOptions.UseInvariantCultureNumberFormat
            //ExSummary:Shows how to format numbers according to the invariant culture.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            Field field = builder.InsertField(" = 1234567,89 \\# $#,###,###.##");
            field.Update();

            // The combination of field, number format and thread culture can sometimes produce an unsuitable result
            Assert.IsFalse(doc.FieldOptions.UseInvariantCultureNumberFormat);
            Assert.AreEqual("$1234567,89 .     ", field.Result);

            // We can set this attribute to avoid changing the whole thread culture just for numeric formats
            doc.FieldOptions.UseInvariantCultureNumberFormat = true;
            field.Update();
            Assert.AreEqual("$1.234.567,89", field.Result);
            //ExEnd
        }

        [Test]
        [Ignore("WORDSNET-17657")]
        public void FieldStyleRefParagraphNumbers()
        {
            //ExStart
            //ExFor:FieldStyleRef
            //ExFor:FieldStyleRef.InsertParagraphNumber
            //ExFor:FieldStyleRef.InsertParagraphNumberInFullContext
            //ExFor:FieldStyleRef.InsertParagraphNumberInRelativeContext
            //ExFor:FieldStyleRef.InsertRelativePosition
            //ExFor:FieldStyleRef.SearchFromBottom
            //ExFor:FieldStyleRef.StyleName
            //ExFor:FieldStyleRef.SuppressNonDelimiters
            //ExSummary:Shows how to use STYLEREF fields.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create a list based on one of the Microsoft Word list templates
            Aspose.Words.Lists.List list = doc.Lists.Add(Aspose.Words.Lists.ListTemplate.NumberDefault);

            // This generated list will look like "1.a )"
            // The space before the bracket is a non-delimiter character that can be suppressed
            list.ListLevels[0].NumberFormat = "\x0000.";
            list.ListLevels[1].NumberFormat = "\x0001 )";

            // Add text and apply paragraph styles that will be referenced by STYLEREF fields
            builder.ListFormat.List = list;
            builder.ListFormat.ListIndent();
            builder.ParagraphFormat.Style = doc.Styles["List Paragraph"];
            builder.Writeln("Item 1");
            builder.ParagraphFormat.Style = doc.Styles["Quote"];
            builder.Writeln("Item 2");
            builder.ParagraphFormat.Style = doc.Styles["List Paragraph"];
            builder.Writeln("Item 3");
            builder.ListFormat.RemoveNumbers();
            builder.ParagraphFormat.Style = doc.Styles["Normal"];

            // Place a STYLEREF field in the header and have it display the first "List Paragraph"-styled text in the document
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            FieldStyleRef fieldStyleRef = (FieldStyleRef)builder.InsertField(FieldType.FieldStyleRef, true);
            fieldStyleRef.StyleName = "List Paragraph";

            // Place a STYLEREF field in the footer and have it display the last text
            builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
            fieldStyleRef = (FieldStyleRef)builder.InsertField(FieldType.FieldStyleRef, true);
            fieldStyleRef.StyleName = "List Paragraph";
            fieldStyleRef.SearchFromBottom = true;

            builder.MoveToDocumentEnd();

            // We can also use STYLEREF fields to reference the list numbers of lists
            builder.Write("\nParagraph number: ");
            fieldStyleRef = (FieldStyleRef)builder.InsertField(FieldType.FieldStyleRef, true);
            fieldStyleRef.StyleName = "Quote";
            fieldStyleRef.InsertParagraphNumber = true;

            builder.Write("\nParagraph number, relative context: ");
            fieldStyleRef = (FieldStyleRef)builder.InsertField(FieldType.FieldStyleRef, true);
            fieldStyleRef.StyleName = "Quote";
            fieldStyleRef.InsertParagraphNumberInRelativeContext = true;

            builder.Write("\nParagraph number, full context: ");
            fieldStyleRef = (FieldStyleRef)builder.InsertField(FieldType.FieldStyleRef, true);
            fieldStyleRef.StyleName = "Quote";
            fieldStyleRef.InsertParagraphNumberInFullContext = true;

            builder.Write("\nParagraph number, full context, non-delimiter chars suppressed: ");
            fieldStyleRef = (FieldStyleRef)builder.InsertField(FieldType.FieldStyleRef, true);
            fieldStyleRef.StyleName = "Quote";
            fieldStyleRef.InsertParagraphNumberInFullContext = true;
            fieldStyleRef.SuppressNonDelimiters = true;

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FieldStyleRef.docx");
            //ExEnd
        }

#if (!__MOBILE__)
        [Test]
        public void FieldDate()
        {
            //ExStart
            //ExFor:FieldDate
            //ExFor:FieldDate.UseLunarCalendar
            //ExFor:FieldDate.UseSakaEraCalendar
            //ExFor:FieldDate.UseUmAlQuraCalendar
            //ExFor:FieldDate.UseLastFormat
            //ExSummary:Shows how to insert DATE fields with different kinds of calendars.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // One way of putting dates into our documents is inserting DATE fields with document builder
            FieldDate fieldDate = (FieldDate)builder.InsertField(FieldType.FieldDate, true);

            // Set the field's date to the current date of the Islamic Lunar Calendar
            fieldDate.UseLunarCalendar = true;
            Assert.AreEqual(" DATE  \\h", fieldDate.GetFieldCode());
            builder.Writeln();

            // Insert a date field with the current date of the Umm al-Qura calendar
            fieldDate = (FieldDate)builder.InsertField(FieldType.FieldDate, true);
            fieldDate.UseUmAlQuraCalendar = true;
            Assert.AreEqual(" DATE  \\u", fieldDate.GetFieldCode());
            builder.Writeln();

            // Insert a date field with the current date of the Indian national calendar
            fieldDate = (FieldDate)builder.InsertField(FieldType.FieldDate, true);
            fieldDate.UseSakaEraCalendar = true;
            Assert.AreEqual(" DATE  \\s", fieldDate.GetFieldCode());
            builder.Writeln();

            // Insert a date field with the current date of the calendar used in the (Insert > Date and Time) dialog box
            fieldDate = (FieldDate)builder.InsertField(FieldType.FieldDate, true);
            fieldDate.UseLastFormat = true;
            Assert.AreEqual(" DATE  \\l", fieldDate.GetFieldCode());
            builder.Writeln();

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Date.docx");
            //ExEnd
        }
#endif

        [Test]
        [Ignore("WORDSNET-17669")]
        public void FieldCreateDate()
        {
            //ExStart
            //ExFor:FieldCreateDate
            //ExFor:FieldCreateDate.UseLunarCalendar
            //ExFor:FieldCreateDate.UseSakaEraCalendar
            //ExFor:FieldCreateDate.UseUmAlQuraCalendar
            //ExSummary:Shows how to insert CREATEDATE fields to display document creation dates.
            // Open an existing document and move a document builder to the end
            Document doc = new Document(MyDir + "Document.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.Writeln(" Date this document was created:");

            // Insert a CREATEDATE field and display, using the Lunar Calendar, the date the document was created
            builder.Write("According to the Lunar Calendar - ");
            FieldCreateDate fieldCreateDate = (FieldCreateDate)builder.InsertField(FieldType.FieldCreateDate, true);
            fieldCreateDate.UseLunarCalendar = true;
            Assert.AreEqual(" CREATEDATE  \\h", fieldCreateDate.GetFieldCode());
            builder.Writeln();

            // Display the date using the Umm al-Qura Calendar
            builder.Write("According to the Umm al-Qura Calendar - ");
            fieldCreateDate = (FieldCreateDate)builder.InsertField(FieldType.FieldCreateDate, true);
            fieldCreateDate.UseUmAlQuraCalendar = true;
            Assert.AreEqual(" CREATEDATE  \\u", fieldCreateDate.GetFieldCode());
            builder.Writeln();

            // Display the date using the Indian National Calendar
            builder.Write("According to the Indian National Calendar - ");
            fieldCreateDate = (FieldCreateDate)builder.InsertField(FieldType.FieldCreateDate, true);
            fieldCreateDate.UseSakaEraCalendar = true;
            Assert.AreEqual(" CREATEDATE  \\s", fieldCreateDate.GetFieldCode());
            builder.Writeln();

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.CreateDate.docx");
            //ExEnd
        }

        [Test]
        public void FieldSaveDate()
        {
            //ExStart
            //ExFor:FieldSaveDate
            //ExFor:FieldSaveDate.UseLunarCalendar
            //ExFor:FieldSaveDate.UseSakaEraCalendar
            //ExFor:FieldSaveDate.UseUmAlQuraCalendar
            //ExSummary:Shows how to insert SAVEDATE fields the date and time a document was last saved.
            // Open an existing document and move a document builder to the end
            Document doc = new Document(MyDir + "Document.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.Writeln(" Date this document was last saved:");

            // Insert a SAVEDATE field and display, using the Lunar Calendar, the date the document was last saved
            builder.Write("According to the Lunar Calendar - ");
            FieldSaveDate fieldSaveDate = (FieldSaveDate)builder.InsertField(FieldType.FieldSaveDate, true);
            fieldSaveDate.UseLunarCalendar = true;
            Assert.AreEqual(" SAVEDATE  \\h", fieldSaveDate.GetFieldCode());
            builder.Writeln();

            // Display the date using the Umm al-Qura Calendar
            builder.Write("According to the Umm al-Qura calendar - ");
            fieldSaveDate = (FieldSaveDate)builder.InsertField(FieldType.FieldSaveDate, true);
            fieldSaveDate.UseUmAlQuraCalendar = true;
            Assert.AreEqual(" SAVEDATE  \\u", fieldSaveDate.GetFieldCode());
            builder.Writeln();

            // Display the date using the Indian National Calendar
            builder.Write("According to the Indian National calendar - ");
            fieldSaveDate = (FieldSaveDate)builder.InsertField(FieldType.FieldSaveDate, true);
            fieldSaveDate.UseSakaEraCalendar = true;
            Assert.AreEqual(" SAVEDATE  \\s", fieldSaveDate.GetFieldCode());
            builder.Writeln();

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.SaveDate.docx");
            //ExEnd
        }

        [Test]
        public void FieldBuilder()
        {
            //ExStart
            //ExFor:FieldBuilder
            //ExFor:FieldBuilder.AddArgument(Int32)
            //ExFor:FieldBuilder.AddArgument(FieldArgumentBuilder)
            //ExFor:FieldBuilder.AddArgument(String)
            //ExFor:FieldBuilder.AddArgument(Double)
            //ExFor:FieldBuilder.AddArgument(FieldBuilder)
            //ExFor:FieldBuilder.AddSwitch(String)
            //ExFor:FieldBuilder.AddSwitch(String, Double)
            //ExFor:FieldBuilder.AddSwitch(String, Int32)
            //ExFor:FieldBuilder.AddSwitch(String, String)
            //ExFor:FieldBuilder.BuildAndInsert(Paragraph)
            //ExFor:FieldArgumentBuilder
            //ExFor:FieldArgumentBuilder.AddField(FieldBuilder)
            //ExFor:FieldArgumentBuilder.AddText(String)
            //ExFor:FieldArgumentBuilder.AddNode(Inline)
            //ExSummary:Shows how to insert fields using a field builder.
            Document doc = new Document();

            // Use a field builder to add a SYMBOL field which displays the "F with hook" symbol
            FieldBuilder builder = new FieldBuilder(FieldType.FieldSymbol);
            builder.AddArgument(402);
            builder.AddSwitch("\\f", "Arial");
            builder.AddSwitch("\\s", 25);
            builder.AddSwitch("\\u");
            Field field = builder.BuildAndInsert(doc.FirstSection.Body.FirstParagraph);

            Assert.AreEqual(" SYMBOL 402 \\f Arial \\s 25 \\u ", field.GetFieldCode());

            // Use a field builder to create a formula field that will be used by another field builder
            FieldBuilder innerFormulaBuilder = new FieldBuilder(FieldType.FieldFormula);
            innerFormulaBuilder.AddArgument(100);
            innerFormulaBuilder.AddArgument("+");
            innerFormulaBuilder.AddArgument(74);

            // Add a field builder as an argument to another field builder
            // The result of our formula field will be used as an ANSI value representing the "enclosed R" symbol,
            // to be displayed by this SYMBOL field
            builder = new FieldBuilder(FieldType.FieldSymbol);
            builder.AddArgument(innerFormulaBuilder);
            field = builder.BuildAndInsert(doc.FirstSection.Body.AppendParagraph(""));

            Assert.AreEqual(" SYMBOL \u0013 = 100 + 74 \u0014\u0015 ", field.GetFieldCode());

            // Now we will use our builder to construct a more complex field with nested fields
            // For our IF field, we will first create two formula fields to serve as expressions
            // Their results will be tested for equality to decide what value an IF field displays
            FieldBuilder leftExpression = new FieldBuilder(FieldType.FieldFormula);
            leftExpression.AddArgument(2);
            leftExpression.AddArgument("+");
            leftExpression.AddArgument(3);

            FieldBuilder rightExpression = new FieldBuilder(FieldType.FieldFormula);
            rightExpression.AddArgument(2.5);
            rightExpression.AddArgument("*");
            rightExpression.AddArgument(5.2);

            // Next, we will create two field arguments using field argument builders
            // These will serve as the two possible outputs of our IF field and they will also use our two expressions
            FieldArgumentBuilder trueOutput = new FieldArgumentBuilder();
            trueOutput.AddText("True, both expressions amount to ");
            trueOutput.AddField(leftExpression);

            FieldArgumentBuilder falseOutput = new FieldArgumentBuilder();
            falseOutput.AddNode(new Run(doc, "False, "));
            falseOutput.AddField(leftExpression);
            falseOutput.AddNode(new Run(doc, " does not equal "));
            falseOutput.AddField(rightExpression);

            // Finally, we will use a field builder to create an IF field which takes two field builders as expressions,
            // and two field argument builders as the two potential outputs
            builder = new FieldBuilder(FieldType.FieldIf);
            builder.AddArgument(leftExpression);
            builder.AddArgument("=");
            builder.AddArgument(rightExpression);
            builder.AddArgument(trueOutput);
            builder.AddArgument(falseOutput);

            builder.BuildAndInsert(doc.FirstSection.Body.AppendParagraph(""));

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FieldBuilder.docx");
            //ExEnd
        }
        
        [Test]
        public void FieldAuthor()
        {
            //ExStart
            //ExFor:FieldAuthor
            //ExFor:FieldAuthor.AuthorName                 
            //ExSummary:Shows how to display a document creator's name with an AUTHOR field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("This document was created by ");
            FieldAuthor fieldAuthor = (FieldAuthor)builder.InsertField(FieldType.FieldAuthor, true);
            
            // If we open an existing document, the document's author's full name will be displayed by the field
            // If we create a document programmatically, we need to set this attribute to the author's name so our field has something to display
            doc.BuiltInDocumentProperties.Author = "John Doe";
            
            fieldAuthor.Update();

            Assert.AreEqual(" AUTHOR ", fieldAuthor.GetFieldCode());
            Assert.AreEqual("John Doe", fieldAuthor.Result);
            
            // Our field can also override the document's built in author name like this
            fieldAuthor.AuthorName = "Jane Doe";
            fieldAuthor.Update();

            Assert.AreEqual(" AUTHOR  \"Jane Doe\"", fieldAuthor.GetFieldCode());
            Assert.AreEqual("Jane Doe", fieldAuthor.Result);
            
            doc.Save(ArtifactsDir + "Field.Author.docx");
            //ExEnd
        }

        [Test]
        public void FieldDoc()
        {
            //ExStart
            //ExFor:FieldDocProperty
            //ExFor:FieldDocVariable
            //ExFor:FieldDocVariable.VariableName
            //ExSummary:Shows how to use fields to display document properties and variables.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set the value of a document property
            doc.BuiltInDocumentProperties.Category = "My category";

            // Display the value of that property with a DOCPROPERTY field
            FieldDocProperty fieldDocProperty = (FieldDocProperty)builder.InsertField(" DOCPROPERTY Category ");
            fieldDocProperty.Update();

            Assert.AreEqual(" DOCPROPERTY Category ", fieldDocProperty.GetFieldCode());
            Assert.AreEqual("My category", doc.BuiltInDocumentProperties.Category);

            builder.Writeln();

            // While the set of a document's properties is fixed, we can add, name and define our own values in the variables collection
            Assert.IsEmpty(doc.Variables);
            doc.Variables.Add("My variable", "My variable's value");

            // We can access a variable using its name and display it with a DOCVARIABLE field
            FieldDocVariable fieldDocVariable = (FieldDocVariable)builder.InsertField(FieldType.FieldDocVariable, true);
            fieldDocVariable.VariableName = "My Variable";
            fieldDocVariable.Update();

            Assert.AreEqual(" DOCVARIABLE  \"My Variable\"", fieldDocVariable.GetFieldCode());
            Assert.AreEqual("My variable's value", fieldDocVariable.Result);

            doc.Save(ArtifactsDir + "Field.DocProperties.docx");
            //ExEnd
        }
        
        [Test]
        public void FieldComments()
        {
            //ExStart
            //ExFor:FieldComments
            //ExFor:FieldComments.Text
            //ExSummary:Shows how to use the COMMENTS field to display a document's comments.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // This property is where the COMMENTS field will source its content from
            doc.BuiltInDocumentProperties.Comments = "My comment.";

            // Insert a COMMENTS field with a document builder
            FieldComments field = (FieldComments)builder.InsertField(FieldType.FieldComments, true);
            field.Update();

            Assert.AreEqual("My comment.", field.Result);

            // We can override the comment from the document's built in properties and display any text we put here instead
            field.Text = "My overriding comment.";
            field.Update();

            Assert.AreEqual("My overriding comment.", field.Result);

            doc.Save(ArtifactsDir + "Field.Comments.docx");
            //ExEnd
        }
        
        [Test]
        public void FieldFileSize()
        {
            //ExStart
            //ExFor:FieldFileSize
            //ExFor:FieldFileSize.IsInKilobytes
            //ExFor:FieldFileSize.IsInMegabytes            
            //ExSummary:Shows how to display the file size of a document with a FILESIZE field.
            Document doc = new Document(MyDir + "Document.doc");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();

            // By default, file size is displayed in bytes
            FieldFileSize field = (FieldFileSize)builder.InsertField(FieldType.FieldFileSize, true);
            field.Update();
            Assert.AreEqual("23040", field.Result);

            // Set the field to display size in kilobytes
            field = (FieldFileSize)builder.InsertField(FieldType.FieldFileSize, true);
            field.IsInKilobytes = true;
            field.Update();
            Assert.AreEqual("23", field.Result);

            // Set the field to display size in megabytes
            field = (FieldFileSize)builder.InsertField(FieldType.FieldFileSize, true);
            field.IsInMegabytes = true;
            field.Update();
            Assert.AreEqual("0", field.Result);
            //ExEnd
        }

        [Test]
        public void FieldGoToButton()
        {
            //ExStart
            //ExFor:FieldGoToButton
            //ExFor:FieldGoToButton.DisplayText
            //ExFor:FieldGoToButton.Location
            //ExSummary:Shows to insert a GOTOBUTTON field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add a GOTOBUTTON which will take us to a bookmark referenced by "MyBookmark"
            FieldGoToButton field = (FieldGoToButton)builder.InsertField(FieldType.FieldGoToButton, true);
            field.DisplayText = "My Button";
            field.Location = "MyBookmark";

            Assert.AreEqual(" GOTOBUTTON  MyBookmark My Button", field.GetFieldCode());

            // Add an arrival destination for our button
            builder.InsertBreak(BreakType.PageBreak);
            builder.StartBookmark(field.Location);
            builder.Writeln("Bookmark text contents.");
            builder.EndBookmark(field.Location);

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.GoToButton.docx");
        }
        
        [Test]
        //ExStart
        //ExFor:FieldFillIn
        //ExFor:FieldFillIn.DefaultResponse
        //ExFor:FieldFillIn.PromptOnceOnMailMerge
        //ExFor:FieldFillIn.PromptText
        //ExSummary:Shows how to use the FILLIN field to prompt the user for a response.
        public void FieldFillIn()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a FILLIN field with a document builder
            FieldFillIn field = (FieldFillIn)builder.InsertField(FieldType.FieldFillIn, true);
            field.PromptText = "Please enter a response:";
            field.DefaultResponse = "A default response";

            // Set this to prompt the user for a response when a mail merge is performed
            field.PromptOnceOnMailMerge = true;

            Assert.AreEqual(" FILLIN  \"Please enter a response:\" \\d \"A default response\" \\o", field.GetFieldCode());

            // Perform a simple mail merge
            FieldMergeField mergeField = (FieldMergeField)builder.InsertField(FieldType.FieldMergeField, true);
            mergeField.FieldName = "MergeField";
            
            doc.FieldOptions.UserPromptRespondent = new PromptRespondent();
            doc.MailMerge.Execute(new [] { "MergeField" }, new object[] { "" });
            
            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.FillIn.docx");
        }

        /// <summary>
        /// IFieldUserPromptRespondent implementation that appends a line to the default response of an FILLIN field during a mail merge
        /// </summary>
        private class PromptRespondent : IFieldUserPromptRespondent
        {
            public string Respond(string promptText, string defaultResponse)
            {
                return "Response from PromptRespondent. " + defaultResponse;
            }
        }
        //ExEnd

        [Test]
        public void FieldInfo()
        {
            //ExStart
            //ExFor:FieldInfo
            //ExFor:FieldInfo.InfoType
            //ExFor:FieldInfo.NewValue
            //ExSummary:Shows how to work with INFO fields.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Set the value of a document property
            doc.BuiltInDocumentProperties.Comments = "My comment";

            // We can access a property using its name and display it with an INFO field
            // In this case it will be the Comments property
            FieldInfo field = (FieldInfo)builder.InsertField(FieldType.FieldInfo, true);
            field.InfoType = "Comments";
            field.Update();

            Assert.AreEqual(" INFO  Comments", field.GetFieldCode());
            Assert.AreEqual("My comment", field.Result);

            builder.Writeln();

            // We can override the value of a document property by setting an INFO field's optional new value
            field = (FieldInfo)builder.InsertField(FieldType.FieldInfo, true);
            field.InfoType = "Comments";
            field.NewValue = "New comment";
            field.Update();

            // Our field's new value has been applied to the corresponding property
            Assert.AreEqual(" INFO  Comments \"New comment\"", field.GetFieldCode());
            Assert.AreEqual("New comment", field.Result);
            Assert.AreEqual("New comment", doc.BuiltInDocumentProperties.Comments);

            doc.Save(ArtifactsDir + "Field.Info.docx");
            //ExEnd
        }

        [Test]
        public void FieldMacroButton()
        {
            //ExStart
            //ExFor:Document.HasMacros
            //ExFor:FieldMacroButton
            //ExFor:FieldMacroButton.DisplayText
            //ExFor:FieldMacroButton.MacroName
            //ExSummary:Shows how to use MACROBUTTON fields that enable us to run macros by clicking.
            // Open a document that contains macros
            Document doc = new Document(MyDir + "Document.HasMacro.docm");
            DocumentBuilder builder = new DocumentBuilder(doc);
            Assert.IsTrue(doc.HasMacros);

            // Insert a MACROBUTTON field and reference by name a macro that exists within the input document
            FieldMacroButton field = (FieldMacroButton)builder.InsertField(FieldType.FieldMacroButton, true);
            field.MacroName = "MyMacro";
            field.DisplayText = "Double click to run macro: " + field.MacroName;

            Assert.AreEqual(" MACROBUTTON  MyMacro Double click to run macro: MyMacro", field.GetFieldCode());

            builder.InsertParagraph();

            // Reference "ViewZoom200", a macro that was shipped with Microsoft Word, found under "Word commands"
            // If our document has a macro of the same name as one from another source, the field will select ours to run
            field = (FieldMacroButton)builder.InsertField(FieldType.FieldMacroButton, true);
            field.MacroName = "ViewZoom200";
            field.DisplayText = "Run " + field.MacroName;

            Assert.AreEqual(" MACROBUTTON  ViewZoom200 Run ViewZoom200", field.GetFieldCode());

            // Save the document as a macro-enabled document type
            doc.Save(ArtifactsDir + "Field.MacroButton.docm");
            //ExEnd
        }
        
        [Test]
        public void FieldKeywords()
        {
            //ExStart
            //ExFor:FieldKeywords
            //ExFor:FieldKeywords.Text
            //ExSummary:Shows to insert a KEYWORDS field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add some keywords, also referred to as "tags" in File Explorer
            doc.BuiltInDocumentProperties.Keywords = "Keyword1, Keyword2";

            // Add a KEYWORDS field which will display our keywords
            FieldKeywords field = (FieldKeywords)builder.InsertField(FieldType.FieldKeyword, true);
            field.Update();

            Assert.AreEqual(" KEYWORDS ", field.GetFieldCode());
            Assert.AreEqual("Keyword1, Keyword2", field.Result);

            // We can set the Text property of our field to display a different value to the one within the document's properties
            field.Text = "OverridingKeyword";
            field.Update();

            Assert.AreEqual(" KEYWORDS  OverridingKeyword", field.GetFieldCode());
            Assert.AreEqual("OverridingKeyword", field.Result);

            // Setting a KEYWORDS field's Text property also updates the document's keywords to our new value
            Assert.AreEqual("OverridingKeyword", doc.BuiltInDocumentProperties.Keywords);

            doc.Save(ArtifactsDir + "Field.Keywords.docx");
        }

        [Test]
        public void FieldNum()
        {
            //ExStart
            //ExFor:FieldPage
            //ExFor:FieldNumChars
            //ExFor:FieldNumPages
            //ExFor:FieldNumWords
            //ExSummary:Shows how to use NUMCHARS, NUMWORDS, NUMPAGES and PAGE fields to track the size of our documents.
            // Open a document to which we want to add character/word/page counts
            Document doc = new Document(MyDir + "Lists.PrintOutAllLists.doc");
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Move the document builder to the footer, where we will store our fields
            builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            // Insert character and word counts
            FieldNumChars fieldNumChars = (FieldNumChars)builder.InsertField(FieldType.FieldNumChars, true);       
            builder.Writeln(" characters");
            FieldNumWords fieldNumWords = (FieldNumWords)builder.InsertField(FieldType.FieldNumWords, true);
            builder.Writeln(" words");

            // Insert a "Page x of y" page count
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            builder.Write("Page ");
            FieldPage fieldPage = (FieldPage)builder.InsertField(FieldType.FieldPage, true);
            builder.Write(" of ");
            FieldNumPages fieldNumPages = (FieldNumPages)builder.InsertField(FieldType.FieldNumPages, true);

            Assert.AreEqual(" NUMCHARS ", fieldNumChars.GetFieldCode());
            Assert.AreEqual(" NUMWORDS ", fieldNumWords.GetFieldCode());
            Assert.AreEqual(" NUMPAGES ", fieldNumPages.GetFieldCode());
            Assert.AreEqual(" PAGE ", fieldPage.GetFieldCode());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Num.docx");
            //ExEnd
        }

        [Test]
        public void FieldPrint()
        {
            //ExStart
            //ExFor:FieldPrint
            //ExFor:FieldPrint.PostScriptGroup
            //ExFor:FieldPrint.PrinterInstructions
            //ExFor:FieldPrintDate
            //ExFor:FieldPrintDate.UseLunarCalendar
            //ExFor:FieldPrintDate.UseSakaEraCalendar
            //ExFor:FieldPrintDate.UseUmAlQuraCalendar
            //ExSummary:Shows to insert a PRINT field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("My paragraph");

            // The PRINT field can send instructions to the printer that we use to print our document
            FieldPrint field = (FieldPrint)builder.InsertField(FieldType.FieldPrint, true);

            // Set the area for the printer to perform instructions over
            // In this case it will be the paragraph that contains our PRINT field
            field.PostScriptGroup = "para";

            // When our document is printed using a printer that supports PostScript,
            // this command will turn the entire area that we specified in field.PostScriptGroup white 
            field.PrinterInstructions = "erasepage";

            Assert.AreEqual(" PRINT  erasepage \\p para", field.GetFieldCode());

            builder.InsertParagraph();

            // PRINTDATE field will display "0/0/0000" by default
            // When a document is printed by a printer or printed as a PDF (but not exported as PDF),
            // these fields will display the date/time of the printing operation, in various calendars
            FieldPrintDate fieldPrintDate = (FieldPrintDate)builder.InsertField(FieldType.FieldPrintDate, true);
            fieldPrintDate.UseLunarCalendar = true;
            builder.Writeln();

            Assert.AreEqual(" PRINTDATE  \\h", fieldPrintDate.GetFieldCode());

            fieldPrintDate = (FieldPrintDate)builder.InsertField(FieldType.FieldPrintDate, true);
            fieldPrintDate.UseSakaEraCalendar = true;
            builder.Writeln();

            Assert.AreEqual(" PRINTDATE  \\s", fieldPrintDate.GetFieldCode());

            fieldPrintDate = (FieldPrintDate)builder.InsertField(FieldType.FieldPrintDate, true);
            fieldPrintDate.UseUmAlQuraCalendar = true;
            builder.Writeln();

            Assert.AreEqual(" PRINTDATE  \\u", fieldPrintDate.GetFieldCode());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Print.docx");
            //ExEnd
        }

        [Test]
        public void FieldQuote()
        {
            //ExStart
            //ExFor:FieldQuote
            //ExFor:FieldQuote.Text
            //ExSummary:Shows to use the QUOTE field.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a QUOTE field, which will display content from the Text attribute
            FieldQuote field = (FieldQuote)builder.InsertField(FieldType.FieldQuote, true);
            field.Text = "\"Quoted text\"";

            Assert.AreEqual(" QUOTE  \"\\\"Quoted text\\\"\"", field.GetFieldCode());

            builder.InsertParagraph();

            // Insert a QUOTE field with a nested DATE field
            // DATE fields normally update their value to the current date every time the document is opened
            // Nesting the DATE field inside the QUOTE field like this will freeze its value to the date when we created the document
            builder.Write("Document creation date: ");
            field = (FieldQuote)builder.InsertField(FieldType.FieldQuote, true);
            builder.MoveTo(field.Separator);
            builder.InsertField(FieldType.FieldDate, true);

            Assert.AreEqual(" QUOTE \u0013 DATE \u0014" + System.DateTime.Now.Date.ToShortDateString() + "\u0015", field.GetFieldCode());

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.Quote.docx");
            //ExEnd
        }

        //ExStart
        //ExFor:FieldNext
        //ExFor:FieldNextIf
        //ExFor:FieldNextIf.ComparisonOperator
        //ExFor:FieldNextIf.LeftExpression
        //ExFor:FieldNextIf.RightExpression
        //ExSummary:Shows how to use NEXT/NEXTIF fields to merge more than one row into one page during a mail merge.
        [Test] //ExSkip
        public void FieldNext()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            // Create a data source for our mail merge with 3 rows,
            // This would normally amount to 3 pages in the output of a mail merge
            DataTable table = new DataTable("Employees");
            table.Columns.Add("Courtesy Title");
            table.Columns.Add("First Name");
            table.Columns.Add("Last Name");
            table.Rows.Add("Mr.", "John", "Doe");
            table.Rows.Add("Mrs.", "Jane", "Cardholder");
            table.Rows.Add("Mr.", "Joe", "Bloggs");

            // Insert a set of merge fields
            InsertMergeFields(builder, "First row: ");

            // If we have multiple merge fields with the same FieldName,
            // they will receive data from the same row of the data source and will display the same value after the merge
            // A NEXT field tells the mail merge instantly to move down one row,
            // so any upcoming merge fields will have data deposited from the next row
            // Make sure not to skip with a NEXT/NEXTIF field while on the last row
            FieldNext fieldNext = (FieldNext)builder.InsertField(FieldType.FieldNext, true);

            // These merge fields are the same as the ones as above but will take values from the second row
            InsertMergeFields(builder, "Second row: ");

            // A NEXTIF field has the same function as a NEXT field,
            // but it skips to the next row only if a condition expressed by the following 3 attributes is fulfilled
            FieldNextIf fieldNextIf = (FieldNextIf)builder.InsertField(FieldType.FieldNextIf, true);
            fieldNextIf.LeftExpression = "5";
            fieldNextIf.RightExpression = "2 + 3";
            fieldNextIf.ComparisonOperator = "=";

            // If the comparison asserted by the above field is correct,
            // the following 3 merge fields will take data from the third row
            // Otherwise, these fields will take data from row 2 again 
            InsertMergeFields(builder, "Third row: ");

            // Our data source has 3 rows and we skipped rows twice, so our output will have one page
            // with data from all 3 rows
            doc.MailMerge.Execute(table);

            Assert.AreEqual(" NEXT ", fieldNext.GetFieldCode());
            Assert.AreEqual(" NEXTIF  5 = \"2 + 3\"", fieldNextIf.GetFieldCode());

            doc.Save(ArtifactsDir + "Field.Next.docx");
        }

        /// <summary>
        /// Uses a document builder to insert merge fields for a data table that has "Courtesy Title", "First Name" and "Last Name" columns
        /// </summary>
        public void InsertMergeFields(DocumentBuilder builder, string firstFieldTextBefore)
        {
            InsertMergeField(builder, "Courtesy Title", firstFieldTextBefore, " ");
            InsertMergeField(builder, "First Name", null, " ");
            InsertMergeField(builder, "Last Name", null, null);
            builder.InsertParagraph();
        }

        /// <summary>
        /// Uses a document builder to insert a merge field
        /// </summary>
        public void InsertMergeField(DocumentBuilder builder, string fieldName, string textBefore, string textAfter)
        {
            FieldMergeField field = (FieldMergeField) builder.InsertField(FieldType.FieldMergeField, true);
            field.FieldName = fieldName;
            field.TextBefore = textBefore;
            field.TextAfter = textAfter;
        }
        //ExEnd
        
        //ExFor:FieldNoteRef
        //ExFor:FieldNoteRef.BookmarkName
        //ExFor:FieldNoteRef.InsertHyperlink
        //ExFor:FieldNoteRef.InsertReferenceMark
        //ExFor:FieldNoteRef.InsertRelativePosition
        //ExSummary:Shows to insert NOTEREF fields and modify their appearance.
        [Test] //ExSkip
        [Ignore("WORDSNET-17845")] //ExSkip
        public void FieldNoteRef()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create a boomkark with a footnote for the NOTEREF field to reference
            InsertBookmarkWithFootnote(builder, "MyBookmark1", "Contents of MyBookmark1", "Footnote from MyBookmark1");

            // This NOTEREF field will display just the number of the footnote inside the referenced bookmark
            // Setting the InsertHyperlink attribute lets us jump to the bookmark by Ctrl + clicking the field
            Assert.AreEqual(" NOTEREF  MyBookmark2 \\h",
                InsertFieldNoteRef(builder, "MyBookmark2", true, false, false, "Hyperlink to Bookmark2, with footnote number ").GetFieldCode());

            // When using the \p flag, after the footnote number the field also displays the position of the bookmark relative to the field
            // Bookmark1 is above this field and contains footnote number 1, so the result will be "1 above" on update
            Assert.AreEqual(" NOTEREF  MyBookmark1 \\h \\p",
                InsertFieldNoteRef(builder, "MyBookmark1", true, true, false, "Bookmark1, with footnote number ").GetFieldCode());

            // Bookmark2 is below this field and contains footnote number 2, so the field will display "2 below"
            // The \f flag makes the number 2 appear in the same format as the footnote number label in the actual text
            Assert.AreEqual(" NOTEREF  MyBookmark2 \\h \\f \\p",
                InsertFieldNoteRef(builder, "MyBookmark2", true, true, true, "Bookmark2, with footnote number ").GetFieldCode());

            builder.InsertBreak(BreakType.PageBreak);
            InsertBookmarkWithFootnote(builder, "MyBookmark2", "Contents of MyBookmark2", "Footnote from MyBookmark2");

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.NoteRef.docx");
        }

        /// <summary>
        /// Uses a document builder to insert a NOTEREF field and sets its attributes
        /// </summary>
        private FieldNoteRef InsertFieldNoteRef(DocumentBuilder builder, string bookmarkName, bool insertHyperlink, bool insertRelativePosition, bool insertReferenceMark, string textBefore)
        {
            builder.Write(textBefore);

            FieldNoteRef field = (FieldNoteRef)builder.InsertField(FieldType.FieldNoteRef, true);
            field.BookmarkName = bookmarkName;
            field.InsertHyperlink = insertHyperlink;
            field.InsertReferenceMark = insertReferenceMark;
            field.InsertRelativePosition = insertRelativePosition;
            builder.Writeln();
            
            return field;
        }
        
        /// <summary>
        /// Uses a document builder to insert a named bookmark with a footnote at the end
        /// </summary>
        private void InsertBookmarkWithFootnote(DocumentBuilder builder, string bookmarkName, string bookmarkText, string footnoteText)
        {
            builder.StartBookmark(bookmarkName);
            builder.Write(bookmarkText);
            builder.InsertFootnote(FootnoteType.Footnote, footnoteText);
            builder.EndBookmark(bookmarkName);
            builder.Writeln();
        }
        //ExEnd

        //ExFor:FieldPageRef
        //ExFor:FieldPageRef.BookmarkName
        //ExFor:FieldPageRef.InsertHyperlink
        //ExFor:FieldPageRef.InsertRelativePosition
        //ExSummary:Shows to insert PAGEREF fields and present them in different ways.
        [Test] //ExSkip
        [Ignore("WORDSNET-17836")] //ExSkip
        public void FieldPageRef()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            InsertAndNameBookmark(builder, "MyBookmark1");

            // This field will display just the page number where the bookmark starts
            // Setting InsertHyperlink attribute makes the field function as a link to the bookmark
            Assert.AreEqual(" PAGEREF  MyBookmark3 \\h", 
                InsertFieldPageRef(builder, "MyBookmark3", true, false, "Hyperlink to Bookmark3, on page: ").GetFieldCode());

            // Setting the \p flag makes the field display the relative position of the bookmark to the field instead of a page number
            // Bookmark1 is on the same page and above this field, so the result will be "above" on update
            Assert.AreEqual(" PAGEREF  MyBookmark1 \\h \\p", 
                InsertFieldPageRef(builder, "MyBookmark1", true, true, "Bookmark1 is ").GetFieldCode());

            // Bookmark2 will be on the same page and below this field, so the field will display "below"
            Assert.AreEqual(" PAGEREF  MyBookmark2 \\h \\p", 
                InsertFieldPageRef(builder, "MyBookmark2", true, true, "Bookmark2 is ").GetFieldCode());

            // Bookmark3 will be on a different page, so the field will display "on page 2"
            Assert.AreEqual(" PAGEREF  MyBookmark3 \\h \\p", 
                InsertFieldPageRef(builder, "MyBookmark3", true, true, "Bookmark3 is ").GetFieldCode());

            InsertAndNameBookmark(builder, "MyBookmark2");
            builder.InsertBreak(BreakType.PageBreak);
            InsertAndNameBookmark(builder, "MyBookmark3");

            doc.UpdateFields();
            doc.Save(ArtifactsDir + "Field.PageRef.docx");
        }

        /// <summary>
        /// Uses a document builder to insert a PAGEREF field and sets its attributes
        /// </summary>
        private FieldPageRef InsertFieldPageRef(DocumentBuilder builder, string bookmarkName, bool insertHyperlink, bool insertRelativePosition, string textBefore)
        {
            builder.Write(textBefore);

            FieldPageRef field = (FieldPageRef)builder.InsertField(FieldType.FieldPageRef, true);
            field.BookmarkName = bookmarkName;
            field.InsertHyperlink = insertHyperlink;
            field.InsertRelativePosition = insertRelativePosition;
            builder.Writeln();
          
            return field;
        }

        /// <summary>
        /// Uses a document builder to insert a named bookmark
        /// </summary>
        private void InsertAndNameBookmark(DocumentBuilder builder, string bookmarkName)
        {
            builder.StartBookmark(bookmarkName);
            builder.Writeln(string.Format("Contents of bookmark \"{0}\".", bookmarkName));
            builder.EndBookmark(bookmarkName);
        }
        //ExEnd
    }
}