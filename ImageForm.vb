Imports System.IO
Public Class ImageForm
    ReadOnly address As String = GrassButtons.address
    Dim fileLocationAllPokemon As String = address & "Pokemon\"
    Dim fileLocationPokemon As String = address & "Target Pokemon\"
    Dim filepathRadarPokemon As String = address & "RadarExclusiveTargetPokemon\"
    Dim fileLocationCheckArea As String = address & "PokemonCheckArea\"
    Private Sub ImageForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBoxPokemon.SelectedItem = "abra"
        targetPokemon = "abra"



        loadPokemon()


        For Each file In Directory.GetFiles(fileLocationPokemon)
            Dim name As String = My.Computer.FileSystem.GetName(file)
            name = name.Replace(".png", "")
            ComboBoxPokemon.Items.Add(name)
        Next
        For Each file In Directory.GetFiles(filepathRadarPokemon)
            Dim name As String = My.Computer.FileSystem.GetName(file)
            name = name.Replace(".png", "")
            ComboBoxPokemon.Items.Add(name)
        Next

        For Each file In Directory.GetFiles(fileLocationAllPokemon)
            Dim name As String = My.Computer.FileSystem.GetName(file)
            name = name.Replace(".png", "")
            If My.Computer.FileSystem.FileExists(fileLocationCheckArea & name & ".txt") Then

                ComboBoxPokemon.Items.Remove(name)
            End If
        Next
    End Sub
    Sub loadPokemon()
        Dim bmp As Bitmap
        If My.Computer.FileSystem.FileExists(fileLocationPokemon & targetPokemon & ".png") Then
            bmp = Bitmap.FromFile(fileLocationPokemon & targetPokemon & ".png")
        Else
            bmp = Bitmap.FromFile(filepathRadarPokemon & targetPokemon & ".png")
        End If
        PictureBox1.Image = CropBitmap(bmp, 1520, 150, 300, 300)
        'Dim stringy As String = My.Computer.FileSystem.ReadAllText(fileLocationCheckArea & targetPokemon & ".txt")
        'Dim x As Integer = CInt(stringy(0) & stringy(1) & stringy(2))
        'Dim y As Integer = CInt(stringy(6) & stringy(7) & stringy(8))
        'xLow = x
        'xHigh = x + 10
        'yLow = y
        'yHigh = y + 10
        '  TurnTargetPixelsBlack()
    End Sub
    Private Function CropBitmap(ByRef bmp As Bitmap, ByVal cropX As Integer, ByVal cropY As Integer, ByVal cropWidth As Integer, ByVal cropHeight As Integer) As Bitmap
        Dim rect As New Rectangle(cropX, cropY, cropWidth, cropHeight)
        Dim cropped As Bitmap = bmp.Clone(rect, bmp.PixelFormat)
        Return cropped
    End Function
    Dim xLow As Integer = 140
    Dim xHigh As Integer = 150
    Dim yLow As Integer = 200
    Dim yHigh As Integer = 210
    Sub TurnTargetPixelsBlack()
        Dim colour As New Color
        colour = Color.Black
        Dim image As Bitmap = PictureBox1.Image

        Dim nii As New Bitmap(image.Width, image.Height, Imaging.PixelFormat.Format32bppArgb)

        For y = 0 To nii.Height - 1
            For x = 0 To nii.Width - 1

                If x > xLow And x < xHigh Then
                    If y > yLow And y < yHigh Then

                        image.SetPixel(x, y, colour)
                    End If
                End If
            Next
        Next
        PictureBox1.Image = image
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        TurnTargetPixelsBlack()
    End Sub
    Dim targetPokemon As String
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxPokemon.SelectedIndexChanged

        targetPokemon = ComboBoxPokemon.Text
        loadPokemon()
    End Sub

    Private Sub ButtonUp_Click(sender As Object, e As EventArgs) Handles ButtonUp.Click
        loadPokemon()
        yLow = yLow - 5
        yHigh = yHigh - 5
        TurnTargetPixelsBlack()
    End Sub
    Private Sub ButtonDown_Click(sender As Object, e As EventArgs) Handles ButtonDown.Click
        loadPokemon()
        yLow = yLow + 5
        yHigh = yHigh + 5
        TurnTargetPixelsBlack()
    End Sub

    Private Sub ButtonRight_Click(sender As Object, e As EventArgs) Handles ButtonRight.Click
        loadPokemon()
        xLow = xLow + 5
        xHigh = xHigh + 5
        TurnTargetPixelsBlack()
    End Sub

    Private Sub ButtonLeft_Click(sender As Object, e As EventArgs) Handles ButtonLeft.Click
        loadPokemon()
        xLow = xLow - 5
        xHigh = xHigh - 5
        TurnTargetPixelsBlack()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ComboBoxPokemon.SelectedIndex = ComboBoxPokemon.SelectedIndex + 1
    End Sub

    Private Sub ButtonSave_Click(Fender As Object, e As EventArgs) Handles ButtonSave.Click
        If My.Computer.FileSystem.FileExists(fileLocationCheckArea & targetPokemon & ".txt") Then
            My.Computer.FileSystem.DeleteFile(fileLocationCheckArea & targetPokemon & ".txt")
        End If
        My.Computer.FileSystem.WriteAllText(fileLocationCheckArea & targetPokemon & ".txt", xLow + 4 & xHigh - 4 & yLow + 4 & yHigh - 4, True)
        Dim stringy As String = My.Computer.FileSystem.ReadAllText(fileLocationCheckArea & targetPokemon & ".txt")


        '   ComboBoxPokemon.SelectedIndex = ComboBoxPokemon.SelectedIndex + 1
    End Sub
End Class