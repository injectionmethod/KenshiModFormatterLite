Imports System.IO
Imports System.IO.Compression

Module Module1
    Dim KENSHI_DAT As String()
    Dim FOLDER_LOCATION As String
    Dim Banner As String = Text.Encoding.Default.GetString(Convert.FromBase64String("IF9fXyAgIF9fXyAgIF9fX19fXyAgIF9fXyAgIF9fICAgIF9fX19fXyAgIF9fXyAgIF9fXyAgICBfX19fX19fXyAgICAgDQovX19fL1wvX18vXCAvX19fX18vXCAvX18vXCAvX18vXCAvX19fX18vXCAvX18vXCAvX18vXCAgL19fX19fX18vXCAgICANClw6Oi5cIFxcIFwgXFw6Ojo6X1wvX1w6OlxfXFwgIFwgXFw6Ojo6X1wvX1w6OlwgXFwgIFwgXCBcX18uOjouX1wvICAgIA0KIFw6OiBcL18pIFwgXFw6XC9fX18vXFw6LiBgLVwgIFwgXFw6XC9fX18vXFw6OlwvX1wgLlwgXCAgIFw6OlwgXCAgICAgDQogIFw6LiBfXyAgKCAoIFw6Ol9fX1wvX1w6LiBfICAgIFwgXFxfOjouX1w6XFw6OiBfX186OlwgXCAgX1w6OlwgXF9fICANCiAgIFw6IFwgKSAgXCBcIFw6XF9fX18vXFwuIFxgLVwgIFwgXCAvX19fX1w6XFw6IFwgXFw6OlwgXC9fX1w6OlxfXy9cIA0KICAgIFxfX1wvXF9fXC8gIFxfX19fX1wvIFxfX1wvIFxfX1wvIFxfX19fX1wvIFxfX1wvIFw6OlwvXF9fX19fX19fXC8gDQotLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tDQogICAgICAgICAgICAgICAgICAgICAgICAgICAgIE1vZCBIYW5kbGVyDQogICAgICAgICAgICAgICAgICAgICAgICAgICAgVmVyc2lvbjogMS4wYQ=="))
    Sub Main()
        Try
            If HandleKenshiDat(False) = True Then
                Console.WriteLine(Banner)
                Console.Write("Enter location of mod zip file:")
                Dim modFile = Console.ReadLine
                For Each f As String In Strings.Split(modFile, ",", -1)
                    ExtractFilesAndFoldersFromZip(f, FOLDER_LOCATION + "/")
                Next
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Console.ReadKey()
        Console.Clear()
        Main()
    End Sub
    Sub ExtractFilesAndFoldersFromZip(zipFilePath As String, outputFolderPath As String)
        Using archive As ZipArchive = ZipFile.OpenRead(zipFilePath)
            Dim firstFolder As String = Path.GetDirectoryName(archive.Entries(0).FullName)

            Dim modEntry As ZipArchiveEntry = archive.Entries.FirstOrDefault(
                Function(entry) entry.FullName.EndsWith(".mod", StringComparison.OrdinalIgnoreCase))

            If modEntry IsNot Nothing Then
                Dim modFolderName As String = Path.GetFileNameWithoutExtension(modEntry.Name)
                Dim modFolderPath As String = Path.Combine(outputFolderPath, modFolderName)
                Console.WriteLine("Installing Mod: " + modFolderName)
                Directory.CreateDirectory(modFolderPath)
                For Each entry As ZipArchiveEntry In archive.Entries
                    Dim entryName As String = entry.FullName.Replace("/", "\")
                    If entryName.StartsWith(firstFolder) AndAlso Not entryName.Equals(firstFolder) Then
                        Dim relativePath As String = entryName.Substring(firstFolder.Length + 1)
                        Dim outputPath As String = Path.Combine(modFolderPath, relativePath)
                        Console.WriteLine("Extracted File: " + entry.Name)
                        If entryName.EndsWith("\") Then
                            If outputFolderPath.Length > 0 Then
                                Directory.CreateDirectory(outputPath) : Console.WriteLine("Created Folder @ " + outputFolderPath)
                            End If
                        Else
                            entry.ExtractToFile(outputPath, True)
                            Console.WriteLine("Created File: " + entry.Name)
                        End If
                    End If
                Next
                Console.WriteLine("Installed Mod: " + modFolderName + vbNewLine)
            End If
        End Using
    End Sub
    Function HandleKenshiDat(b As Boolean) As Boolean
        If IO.File.Exists(Environment.CurrentDirectory + "/Kenshi.dat") Then
            KENSHI_DAT = IO.File.ReadAllLines(Environment.CurrentDirectory + "/Kenshi.dat")
            For Each f In KENSHI_DAT
                If f.StartsWith("Folder=") Then
                    If IO.Directory.Exists(f.Split("=")(1)) Then
                        FOLDER_LOCATION = f.Split("=")(1)
                        b = True
                    Else
                        b = False
                    End If
                End If
            Next
        Else
            b = False
            CreateKenshiDat()
        End If
        Return b
    End Function
    Sub CreateKenshiDat()
        Console.Write("Please Enter The Location Of Your Kenshi Mods Folder:")
        Dim DirectoryOfModFolder = Console.ReadLine
        If Directory.Exists(DirectoryOfModFolder) Then
            IO.File.WriteAllText(Environment.CurrentDirectory + "/Kenshi.dat", $"Folder={DirectoryOfModFolder}")
            Console.WriteLine("Location Wrote To DAT File, Press Any Key To Continue.")
        Else
            Console.WriteLine("The Directory Selected Does Not Exist.")
            CreateKenshiDat()
        End If
    End Sub
End Module
