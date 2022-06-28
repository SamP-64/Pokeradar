Imports System.IO
Public Class GrassButtons
    Public ButtonGrid(15, 15) As Button
    Public address As String = Path.GetFullPath(Application.StartupPath)
    Private Sub GrassButtons_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' address = address.Replace("PokeradarShinyHunter", "PokeradarShinyHunter34543")
        '  address = address.Replace("34543", "\Resources\")  'Download Version

        address = address.Replace("bin\Debug", "34543")
        address = address.Replace("34543", "\Resources\")  'My Computer

        filepathRoutes = address & "Routes\"
        filepathPokemon = address & "Target Pokemon\"
        filepathRadarPokemon = address & "RadarExclusiveTargetPokemon\"
        fileLocationCheckArea = address & "PokemonCheckArea\"

        For i = 1 To 15
            For j = 1 To 15
                Dim button1 As New Button
                Controls.Add(button1)
                button1.Text = ("-")
                button1.BackColor = Color.White
                button1.Location = New Point((20 * i), 20 * j)
                button1.Visible = True
                button1.Width = 20
                button1.Height = 20
                ButtonGrid(i, j) = button1
                AddHandler button1.Click, AddressOf buttonClick
            Next
        Next


        For Each file In Directory.GetFiles(filepathRoutes)
            Dim name As String = My.Computer.FileSystem.GetName(file)
            name = name.Replace(".txt", "")
            ComboBoxLocation.Items.Add(name)
        Next


        For Each file In Directory.GetFiles(filepathPokemon)
            Dim name As String = My.Computer.FileSystem.GetName(file)
            name = name.Replace(".png", "")
            ComboBoxPokemon.Items.Add(name)
        Next

        GetLastValues()

    End Sub
    Sub GetLastValues()
        ComboBoxPokemon.Text = My.Computer.FileSystem.ReadAllText(address & "LastPokemon.txt")
        UpdatePokemonBox()

        ComboBoxLocation.Text = My.Computer.FileSystem.ReadAllText(address & "LastLocation.txt")
        UpdateLocationBox()

        Dim lastposition As String = My.Computer.FileSystem.ReadAllText(address & "LastStartPosition.txt")

        startx = Val(lastposition(0))
        starty = Val(lastposition(1))
        ButtonGrid(startx, starty).BackColor = Color.Cyan
    End Sub
    Protected Sub buttonClick(sender As Object, e As EventArgs)
        If routeName <> "" Then
            GetPatchesFromFile()
        End If
        Dim button1 As Button
        button1 = sender

        If button1.BackColor = Color.LightGreen Then
            button1.BackColor = Color.Cyan
        End If
    End Sub
    Public highestX As Integer
    Public highestY As Integer
    Public Sub ButtonSubmit_Click(sender As Object, e As EventArgs) Handles ButtonSubmit.Click
        CheckArea()
        Dim enteredStart As Boolean = False
        For i = 1 To 15
            For j = 1 To 15
                If ButtonGrid(i, j).BackColor = Color.LightGreen Then
                    '  My.Computer.FileSystem.WriteAllText(filename, i & "," & j & " ", True)
                    If i > highestX Then
                        highestX = i
                    End If
                    If j > highestY Then
                        highestY = j
                    End If

                End If
                If ButtonGrid(i, j).BackColor = Color.Cyan Then
                    startx = i
                    starty = j
                    enteredStart = True
                End If
            Next
        Next

        If ComboBoxLocation.Text = "" Then
            MsgBox("Choose a Location")
            Exit Sub
        End If
        If ComboBoxPokemon.Text = "" Then
            MsgBox("Choose a Target Pokemon")
            Exit Sub
        End If
        If enteredStart = False Then
            MsgBox("Enter Start Position")
            Exit Sub
        End If

        My.Computer.FileSystem.DeleteFile(address & "LastPokemon.txt")
        My.Computer.FileSystem.WriteAllText(address & "LastPokemon.txt", targetPokemon, True)

        My.Computer.FileSystem.DeleteFile(address & "LastLocation.txt")
        My.Computer.FileSystem.WriteAllText(address & "LastLocation.txt", routeName, True)

        My.Computer.FileSystem.DeleteFile(address & "LastStartPosition.txt")
        My.Computer.FileSystem.WriteAllText(address & "LastStartPosition.txt", startx & starty, True)

        Pokeradar.Visible = True
    End Sub
    Public startx As Integer
    Public starty As Integer
    '  ReadOnly address As String = "C:\Users\Sampa\source\repos\Pokeradar2\Pokeradar2\Resources\"
    Dim filepathRoutes As String
    Dim filepathPokemon As String
    Dim filepathRadarPokemon As String
    Dim routeName As String
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        For i = 1 To 15
            For j = 1 To 15
                If i <= CInt(TextBoxGreenX.Text) And j <= CInt(TextBoxGreenY.Text) Then
                    ButtonGrid(i, j).BackColor = Color.LightGreen
                End If
            Next
        Next
    End Sub
    Public TrophyGarden As Boolean = False
    Sub GetPatchesFromFile()
        Dim stringy As String = My.Computer.FileSystem.ReadAllText(filepathRoutes & routeName & ".txt")
        For i = 1 To 15
            For j = 1 To 15
                If stringy.Contains(" " & i & "," & j & " ") Then
                    ButtonGrid(i, j).BackColor = Color.LightGreen
                End If
            Next
        Next
    End Sub
    Public targetPokemon As String
    Public ability As Boolean
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxPokemon.SelectedIndexChanged
        UpdatePokemonBox()
    End Sub
    Sub UpdatePokemonBox()
        targetPokemon = ComboBoxPokemon.Text
        If targetPokemon = "Tauros" Or targetPokemon = "Stantler" Or targetPokemon = "Banette" Or targetPokemon = "Kirlia" Or targetPokemon = "Shinx" Or targetPokemon = "Staravia" Or targetPokemon = "Luxio" Or targetPokemon = "Ralts" Or targetPokemon = "Snover" Then
            ability = True
        Else
            ability = False
        End If
    End Sub
    Private Sub SavePatches_Click_1(sender As Object, e As EventArgs) Handles SavePatches.Click
        routeName = TextBoxRouteName.Text
        For i = 1 To 15
            For j = 1 To 15
                If ButtonGrid(i, j).BackColor = Color.LightGreen Then
                    My.Computer.FileSystem.WriteAllText(filepathRoutes & routeName & ".txt", " " & i & "," & j & " ", True)


                End If
            Next
        Next
    End Sub

    Private Sub ButtonLoadPatches_Click(sender As Object, e As EventArgs) Handles ButtonLoadPatches.Click
        ClearGrid()
        routeName = TextBoxRouteName.Text
        GetPatchesFromFile()
    End Sub
    Sub ClearGrid()

        For i = 1 To 15
            For j = 1 To 15
                ButtonGrid(i, j).BackColor = Color.White
            Next
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ClearGrid()
    End Sub

    Private Sub ComboBoxLocation_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxLocation.SelectedIndexChanged
        UpdateLocationBox()
    End Sub
    Public Sub UpdateLocationBox()
        ClearGrid()
        routeName = ComboBoxLocation.Text
        GetPatchesFromFile()

        If routeName = "Trophy Garden" Then
            TrophyGarden = True
        End If

    End Sub
    Public radarExclusive As Boolean
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            radarExclusive = True
        Else
            radarExclusive = False
        End If

        Dim path As String
        If radarExclusive = True Then
            path = filepathRadarPokemon
        ElseIf radarExclusive = False Then
            path = filepathPokemon
        End If
        ComboBoxPokemon.Items.Clear()

        For Each file In Directory.GetFiles(path)
            Dim name As String = My.Computer.FileSystem.GetName(file)
            name = name.Replace(".png", "")
            ComboBoxPokemon.Items.Add(name)
        Next
    End Sub
    Dim fileLocationCheckArea As String
    Dim x As Integer
    Dim y As Integer
    Sub CheckArea()
        Dim stringy As String = My.Computer.FileSystem.ReadAllText(fileLocationCheckArea & targetPokemon & ".txt")
        x = CInt(stringy(0) & stringy(1) & stringy(2))
        y = CInt(stringy(6) & stringy(7) & stringy(8))
    End Sub
    Public Function GetX()
        Return x + 1520

    End Function
    Public Function GetY()
        Return y + 150
    End Function
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ImageForm.Visible = True
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Process.Start(address & "Instructions.txt")
    End Sub
End Class