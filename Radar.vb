Imports System.IO
Imports System.Runtime.InteropServices
Public Class Pokeradar
    Public GridSizeX As Integer
    Public GridSizeY As Integer
    Public Grid(GrassButtons.highestX, GrassButtons.highestY) As gridy
    Dim picNames(40) As String
    Dim images(40) As Bitmap
    Dim pixelcolours(3) As Color
    Dim count As Integer = 0
    Dim stepCount As Integer = 250
    Dim chainCount As Integer = 0
    Dim highestChain As Integer = 0
    Dim stepsUntilReset As Integer = 0
    Dim resetCount As Integer = 0
    Dim numOfBreaks As Integer = 0
    Dim address As String = GrassButtons.address
    ReadOnly fileLocation As String = address & "Pokeradar\"
    ReadOnly fileLocationColours As String = address & "Colours\"
    ReadOnly fileLocationPokemon As String = address & "Pokemon\"
    ReadOnly fileLocationCheckArea As String = address & "PokemonCheckArea\"
    Dim GPs(16) As Color
    Dim GIs(16) As Bitmap
    Dim SIs(14) As Bitmap
    Dim SPs(14) As Color
    Dim RI As Bitmap = New Bitmap(fileLocationColours & "Red.png")
    Dim RP As Color = RI.GetPixel(1, 1)
    Dim MoveToX As Integer
    Dim MoveToY As Integer
    Public XPos As Integer
    Public YPos As Integer
    Dim foundShiny As Boolean = False
    Dim TargetPokemon As String
    Dim TargetPokemonImage As Bitmap
    Public ability As Boolean
    Public chooseFlash As Boolean
    Dim foundPokemon As String
    Dim foundPokemonImage As Bitmap
    Dim chainStartTime As New Date
    Dim chainEndTime As New Date
    Private Sub Radar_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        DeleteImages()
        GrassButtons.Hide()
        LabelCurrently.Text = "Ready to Start!"

        StartVariables()
        CreateGrid()
        StoreGreens()
    End Sub
    Sub StartVariables()
        GridSizeX = GrassButtons.highestX
        GridSizeY = GrassButtons.highestY
        XPos = GrassButtons.startx
        YPos = GrassButtons.starty
        TargetPokemon = GrassButtons.targetPokemon
        LabelTargetPokemon.Text = "Target: " & TargetPokemon
        ability = GrassButtons.ability
        chooseFlash = GrassButtons.radarExclusive
        TargetPokemonImage = New Bitmap(fileLocationPokemon & TargetPokemon & ".png")

    End Sub
    Private Sub RadarResetTimer_Tick(sender As Object, e As EventArgs) Handles RadarResetTimer.Tick
        Cursor.Position = New Point(900, 790)
        PerformMouseClick("LClick")

        MainSub()
    End Sub
    Sub MainSub()
        RadarResetTimer.Enabled = False
        ResetGrid()
        UseRadar()
        StoreImages()
        Do
            WhatPatchesShook()
            FindMovePosition()
            SearchPath()
            EnterEncounter()
            ResetGrid()
            TakeScreenshotsAfterEncounter()
        Loop
    End Sub
    Sub UseRadar()
        LabelFoundPokemon.Text = "-----"
        LabelCurrently.Text = "Using the Pokeradar"
        LabelCurrently.Refresh()
        wait(0.5)
        count = count + 1
        resetCount = resetCount + 1
        LabelNumOfResets.Text = "Times Used Radar: " & resetCount
        stepsUntilReset = 50
        LabelStepsUntilReset.Text = "Steps Until Reset: " & stepsUntilReset
        Screenshot("BeforeReset " & count)
        picNames(0) = "BeforeReset " & count
        HoldKeyDown(89, 0.05)
        wait(0.109)
        For num = 1 To 3
            TakeScreenshots(num)
        Next
        wait(0.5)
    End Sub
    Sub TakeScreenshots(num As Integer)
        Screenshot(num & "AfterReset " & count)
        picNames(num) = num & "AfterReset " & count
        wait(0.019)
    End Sub
    Sub Screenshot(name As String)
        Using bnp As New Bitmap(1920, 1080)
            Using g As Graphics = Graphics.FromImage(bnp)
                g.CopyFromScreen(0, 0, 0, 0, bnp.Size)
                bnp.Save(fileLocation & name & ".png")
            End Using
        End Using
    End Sub
    Sub StoreImages()
        For i = 0 To 3
            images(i) = New Bitmap(fileLocation & picNames(i) & ".png")
        Next
    End Sub
    Sub WhatPatchesShook()
        For x = 1170 To 1719
            For y = 241 To 700
                If GrassButtons.TrophyGarden = False Then
                    If x Mod 61 > 50 Or x Mod 61 < 29 Then
                    Else
                        If y Mod 51 > 23 And y Mod 51 < 39 Then 'dont edit
                        Else
                            StoreColours(x, y)
                            WhereArePatches(x, y)
                        End If
                    End If
                ElseIf GrassButtons.TrophyGarden = True Then

                    If x Mod 61 > 49 Or x Mod 61 < 22 Then
                    Else
                        If y Mod 51 > 28 And y Mod 51 < 44 Then
                        Else
                            StoreColours(x, y)
                            WhereArePatches(x, y)
                        End If
                    End If

                End If
            Next
        Next
        CheckAbovePatch()
    End Sub
    Sub CheckAbovePatch()
        If Grid(XPos + 1, YPos + 1).Text = "-" And Grid(XPos - 1, YPos - 1).Text = "-" And Grid(XPos, YPos + 1).Text = "-" And Grid(XPos - 1, YPos + 1).Text = "-" And Grid(XPos + 1, YPos - 1).Text = "-" And Grid(XPos, YPos - 1).Text = "-" And Grid(XPos + 1, YPos).Text = "-" And Grid(XPos - 1, YPos).Text = "-" Then
            Grid(XPos, YPos - 1).Text = "X"
        End If
    End Sub
    Sub StoreColours(x As Integer, y As Integer)
        For i = 0 To pixelcolours.Length - 1
            pixelcolours(i) = images(i).GetPixel(x, y)
        Next
    End Sub
    Sub WhereArePatches(x As Integer, y As Integer)
        If pixelcolours(0).ToArgb() <> pixelcolours(1).ToArgb() Then
            SetGridPatches(WhatXPatch(x), WhatYPatch(y), CheckFlashTest(), CheckShinyPatch())
        End If
    End Sub
    Function WhatXPatch(x As Integer)
        Dim newx As Integer
        If 1188 <= x And x <= 1215 Then
            newx = XPos - 4
        ElseIf 1249 <= x And x <= 1276 Then
            newx = XPos - 3
        ElseIf 1310 <= x And x <= 1337 Then
            newx = XPos - 2
        ElseIf 1371 <= x And x <= 1398 Then
            newx = XPos - 1
        ElseIf 1432 <= x And x <= 1459 Then
            newx = XPos
        ElseIf 1493 <= x And x <= 1520 Then
            newx = XPos + 1
        ElseIf 1554 <= x And x <= 1581 Then
            newx = XPos + 2
        ElseIf 1615 <= x And x <= 1642 Then
            newx = XPos + 3
        ElseIf 1676 <= x And x <= 1703 Then
            newx = XPos + 4
        End If
        Return newx
    End Function
    Function WhatYPatch(y As Integer)
        Dim newy As Integer
        If 243 <= y And y <= 278 Then
            newy = YPos - 4
        ElseIf 294 <= y And y <= 329 Then
            newy = YPos - 3
        ElseIf 345 <= y And y <= 380 Then
            newy = YPos - 2
        ElseIf 396 <= y And y <= 431 Then
            newy = YPos - 1
        ElseIf 447 <= y And y <= 482 Then
            newy = YPos
        ElseIf 498 <= y And y <= 533 Then
            newy = YPos + 1
        ElseIf 549 <= y And y <= 584 Then
            newy = YPos + 2
        ElseIf 600 <= y And y <= 635 Then
            newy = YPos + 3
        ElseIf 651 <= y And y <= 686 Then
            newy = YPos + 4
        End If
        Return newy
    End Function
    Function CheckFlashTest()
        Dim curr As Date = Date.Now
        Dim starttime As New Date(curr.Year, curr.Month, curr.Day, 18, 0, 0)
        Dim endtime As New Date(curr.Year, curr.Month, curr.Day, 19, 0, 0)

        If curr >= starttime And curr <= endtime Then
            GPs(5) = Nothing '
        Else
            GPs(5) = GIs(5).GetPixel(1, 1)
        End If

        For Each green As Color In GPs
            If green.ToArgb() = pixelcolours(1).ToArgb() Or green.ToArgb() = pixelcolours(2).ToArgb() Or green.ToArgb() = pixelcolours(3).ToArgb() Then

                Return True
            End If
        Next
        Return False
    End Function
    Function CheckShinyPatch()
        For Each ShinyColor As Color In SPs
            If ShinyColor.ToArgb() = pixelcolours(1).ToArgb() Or ShinyColor.ToArgb() = pixelcolours(2).ToArgb() Or ShinyColor.ToArgb() = pixelcolours(3).ToArgb() Then
                Return True
            End If
        Next
        Return False
    End Function
    Sub FindMovePosition()

        HasErrorOccured()

        MoveToX = 0
        MoveToY = 0
        Dim triggered As Boolean = False
        Dim shiny As Boolean = False
        Dim letter As String
        If chooseFlash = True Then
            letter = "F"
        Else
            letter = "X"
        End If

        For i = 1 To GridSizeX
            For j = 1 To GridSizeY
                If Grid(i, j).Text = letter And Grid(i, j).isSurrounded = True Then
                    If triggered = False Then
                        If chainCount > 0 Then
                            If i = XPos - 4 Or i = XPos + 4 Or j = YPos - 4 Or j = YPos + 4 Then
                                MoveToX = i
                                MoveToY = j
                                triggered = True

                            End If
                        ElseIf chainCount = 0 Then
                            If i = XPos And j = YPos - 1 Then
                            Else

                                MoveToX = i
                                MoveToY = j
                                triggered = True
                            End If
                        End If
                    End If
                End If
            Next
        Next


        For i = 1 To GridSizeX
            For j = 1 To GridSizeY
                If Grid(i, j).Text = "S" Then

                    MoveToX = i
                    MoveToY = j
                    triggered = True
                    shiny = True
                    foundShiny = True
                End If
            Next
        Next
        If shiny = False Then
            If chainCount > 39 Then
                triggered = False
                LabelCurrently.Text = "40 Chain Achieved - Looking For Shiny Patch"
                LabelCurrently.Refresh()
            End If

        End If
        If triggered = False Then
            No4Patches()
        Else
            LabelCurrently.Text = "Moving to Patch"
            LabelCurrently.Refresh()
        End If
        LabelTargetPatch.Text = "Target Patch " & MoveToX & ", " & MoveToY
        Grid(MoveToX, MoveToY).Text = "T"
    End Sub
    Sub HasErrorOccured()
        Dim counter As Integer = 0
        For i = 1 To GridSizeX
            For j = 1 To GridSizeY
                If Grid(i, j).Text = "X" Or Grid(i, j).Text = "F" Or Grid(i, j).Text = "S" Then
                    counter = counter + 1
                End If
            Next
        Next
        If counter > 5 Then
            Screenshot("Chain Broke Due to Error")
            ChainBreak()
        End If
    End Sub
    Sub Moove(key As Integer)
        ' wait(0.359)
        wait(0.1)
        stepCount = stepCount - 1
        LabelStepCount.Text = ("Steps Until Repel: " & stepCount)
        stepsUntilReset = stepsUntilReset - 1
        If stepsUntilReset < 0 Then
            stepsUntilReset = 0
        End If
        LabelStepsUntilReset.Text = "Steps Until Reset: " & stepsUntilReset

        Grid(XPos, YPos).Text = "-"
        If key = 85 Then ' u
            YPos = YPos - 1
        ElseIf key = 74 Then ' j
            YPos = YPos + 1
        ElseIf key = 72 Then ' h
            XPos = XPos - 1
        ElseIf key = 75 Then ' k
            XPos = XPos + 1
        End If
        LabelXPos.Text = "X Position: " & XPos
        LabelYPos.Text = "Y Position: " & YPos
        Grid(XPos, YPos).Text = "P"

        HoldKeyDown(key, 0.2)

        If stepCount = 0 Then
            UseMaxRepel()
            stepCount = 250
        End If
    End Sub
    Sub EnterEncounter()

        LabelCurrently.Text = "Entering Encounter"
        LabelCurrently.Refresh()
        wait(14.509)
        If ability = True Then
            wait(5)
        End If
        If numOfBreaks = 0 And chainCount = 0 And TargetPokemon = "Random" Then
            Screenshot(TargetPokemon)
        End If
        Screenshot(TargetPokemon & " " & chainCount + 1)

        If foundShiny = False Then
            WhatWasFound()
        End If

        chainCount = chainCount + 1
        LabelChainCount.Text = "Chain: " & chainCount
        LabelChainCount.Refresh()


        CheckHighestChain()
        HasShinyPatchBeenFound()
        DefeatPokemon()
        BattleEnd()
    End Sub
    Sub CheckHighestChain()
        If chainCount > highestChain Then
            highestChain = chainCount
            LabelHighestChain.Text = "Highest Chain: " & highestChain
            LabelHighestChain.Refresh()
        End If
    End Sub
    Sub HasShinyPatchBeenFound()
        If foundShiny = True Then
            LabelCurrently.Text = "Shiny Found"
            LabelCurrently.Refresh()

            Dim curr As Date = Date.Now
            chainEndTime = New Date(curr.Year, curr.Month, curr.Day, curr.Hour, curr.Minute, curr.Second)

            Dim time As TimeSpan = chainEndTime - chainStartTime
            MsgBox("Shiny Found!!!!")
            MsgBox("Time " & time.Hours & " : " & time.Minutes & " : " & time.Seconds)
            End
        End If
    End Sub
    Sub DefeatPokemon()
        LabelCurrently.Text = "Defeating Pokemon"
        LabelCurrently.Refresh()
        PerformMouseClick("LClick")
        wait(0.3)
        PerformMouseClick("LClick")
    End Sub
    Sub BattleEnd()
        If chainCount <> 30 Then
            wait(8.7)
        Else
            LabelCurrently.Text = "Using Leppa Berry"
            LabelCurrently.Refresh()
            wait(12)
        End If
    End Sub
    Sub TakeScreenshotsAfterEncounter()
        LabelCurrently.Text = "Using the Radar After the Encounter"
        LabelCurrently.Refresh()
        count = count + 1
        resetCount = resetCount + 1
        LabelNumOfResets.Text = "Times Used Radar: " & resetCount

        Dim finalShot As Integer
        Dim redFound As Boolean = False
        For i = 1 To picNames.Length - 1

            picNames(i) = i + 1 & "AfterReset " & count
            Screenshot("Test" & picNames(i))
            images(i) = New Bitmap(fileLocation & "Test" & picNames(i) & ".png")
            wait(0.099)
            If redFound = False Then
                If images(i).GetPixel(1700, 900) = RP Then
                    finalShot = i + 3
                    redFound = True
                End If
            End If
            If i = finalShot Then
                For num = 1 To 3
                    StoreTestScreenshots(finalShot - 2, num)
                Next
                Exit For
            End If
        Next
        wait(0.5)
        picNames(0) = "BeforeReset " & count
        Screenshot(picNames(0))
        images(0) = New Bitmap(fileLocation & picNames(0) & ".png")

    End Sub
    Sub StoreTestScreenshots(i As Integer, num As Integer)
        images(num) = images(i + num - 1)
        images(num).Save(fileLocation & picNames(num) & ".png")
        My.Computer.FileSystem.RenameFile(fileLocation & picNames(num) & ".png", num & "AfterReset " & count & ".png")
        picNames(num) = num & "AfterReset " & count
    End Sub
    Function Are2ImagesDifferent(image, image2)
        Dim samenum As Integer = 0
        Dim diffnum As Integer = 0
        For x = 1170 To 1719
            For y = 241 To 700
                If x Mod 61 > 1 And x Mod 61 < 23 Then
                Else
                    If y Mod 51 > 23 And y Mod 51 < 39 Then
                    Else
                        Dim pixelcolour As Color = image.GetPixel(x, y)
                        Dim pixelcolour2 As Color = image2.GetPixel(x, y)
                        If pixelcolour.ToArgb() = pixelcolour2.ToArgb() Then
                            samenum = samenum + 1
                        Else
                            diffnum = diffnum + 1

                        End If
                    End If
                End If


            Next
        Next
        If diffnum = 0 Then
            Return False
        Else
            Return True
        End If
    End Function
    Sub No4Patches()

        If chainCount = 0 Then
            SoftReset()
            MainSub()
        End If
        LabelCurrently.Text = "No Suitable Patches - Walking 50 steps"
        LabelCurrently.Refresh()
        If Grid(GrassButtons.startx, GrassButtons.starty).Text = "-" Then
            Grid(GrassButtons.startx, GrassButtons.starty).Text = "T"
        ElseIf Grid(GrassButtons.startx + 1, GrassButtons.starty).Text = "-" Then
            Grid(GrassButtons.startx + 1, GrassButtons.starty).Text = "T"
        ElseIf Grid(GrassButtons.startx, GrassButtons.starty + 1).Text = "-" Then
            Grid(GrassButtons.startx, GrassButtons.starty + 1).Text = "T"
        End If
        LabelTargetPatch.Text = "No Target Patch "
        SearchPath()

        If Grid(XPos - 1, YPos).Text = "-" Then
            Take50Steps(72, 75)
        ElseIf Grid(XPos - 1, YPos).Text = "-" Then
            Take50Steps(75, 72)
        ElseIf Grid(XPos, YPos + 1).Text = "-" Then
            Take50Steps(74, 85)
        ElseIf Grid(XPos, YPos - 1).Text = "-" Then
            Take50Steps(85, 74)
        End If
        wait(1)
        MainSub()
    End Sub
    Sub Take50Steps(key As Integer, key2 As Integer)

        Dim one As Boolean = True
        For i = 1 To stepsUntilReset
            If one = True Then
                Moove(key)
                one = False
            ElseIf one = False Then
                Moove(key2)
                one = True
            End If
        Next

    End Sub
    Sub SetGridPatches(x, y, flashy, shiny)
        If x <= GridSizeX And y <= GridSizeY And x >= 1 And y >= 1 Then

            If Grid(x, y).Text = "X" Or Grid(x, y).Text = "-" Then

                If flashy = False Then
                    Grid(x, y).Text = "X"
                ElseIf flashy = True Then
                    Grid(x, y).Text = "F"
                End If

                If shiny = True Then
                    Grid(x, y).Text = "S"
                End If
            End If
        End If
    End Sub
    Sub UseMaxRepel()
        LabelCurrently.Text = "Using Max Repel"
        LabelCurrently.Refresh()
        wait(4.5)
        HoldKeyDown(65, 0.2) ' a
        wait(2.7)
        HoldKeyDown(88, 1) ' x
        wait(1.4)
        HoldKeyDown(65, 0.2)
        wait(1.7)
        HoldKeyDown(65, 0.2)
        wait(1.7)
        HoldKeyDown(65, 0.2)
        wait(1.7)
        HoldKeyDown(66, 0.2)
        wait(1.7)
        HoldKeyDown(66, 0.2)
        wait(1.7)
        HoldKeyDown(66, 0.2)
        wait(2.7)
    End Sub
    Private Sub ButtonStart_Click(sender As Object, e As EventArgs) Handles ButtonStart.Click
        Dim curr As Date = Date.Now
        chainStartTime = New Date(curr.Year, curr.Month, curr.Day, curr.Hour, curr.Minute, curr.Second)
       
        RadarResetTimer.Enabled = True
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim colour As New Color
        colour = Color.Black
        Dim image As Bitmap = New Bitmap(fileLocationColours & "patches" & ".png")

        Dim nii As New Bitmap(image.Width, image.Height, Imaging.PixelFormat.Format32bppArgb)

        For y = 0 To nii.Height - 1
            For x = 0 To nii.Width - 1
                If y < 241 Or y > 700 Then '51 each
                    image.SetPixel(x, y, colour)
                ElseIf y Mod 51 > 28 And y Mod 51 < 44 Then
                    image.SetPixel(x, y, colour)
                End If
                If x < 1170 Or x > 1719 Then '61 each
                    image.SetPixel(x, y, colour)
                ElseIf x Mod 61 > 48 Or x Mod 61 < 31 Then
                    image.SetPixel(x, y, colour)
                End If

                'If x < 970 Or x > 1150 Then
                '    image.SetPixel(x, y, colour)
                'End If
                'If y < 220 Or y > 222 Then
                '    image.SetPixel(x, y, colour)
                'End If

                'If x < 1667 Or x > 1672 Then
                '    image.SetPixel(x, y, colour)
                'End If
                'If y < 365 Or y > 370 Then
                '    image.SetPixel(x, y, colour)
                'End If
            Next
        Next
        image.Save(fileLocationColours & "123456" & " Testo.png")
    End Sub
    Sub SoftReset()
        HoldKeyDown(84, 1) ' t
        wait(9)
        HoldKeyDown(65, 1) ' a
        wait(1.9)
        HoldKeyDown(65, 1) ' a
        wait(2.4)
        HoldKeyDown(65, 1) ' a
        wait(1.2)
        HoldKeyDown(66, 1) ' b
        wait(1)
        HoldKeyDown(88, 0.3) ' x
        wait(0.3)
        HoldKeyDown(74, 0.2) ' down
        wait(0.3)
        HoldKeyDown(74, 0.2) ' down
        wait(0.3)
        HoldKeyDown(66, 1) ' b
        wait(0.2)
    End Sub
    Sub ChainBreak()
        WhatBrokeTheChain()
        ResetVariables()
        SoftReset()
        MainSub()
    End Sub
    Sub WhatBrokeTheChain()
        If chainCount > 0 Then
            numOfBreaks = numOfBreaks + 1
            LabelSoftResetNum.Text = "Chain Breaks: " & numOfBreaks
            LabelCurrently.Text = "Chain Broke - Resetting"
            Screenshot(foundPokemon & " Broke the Chain " & count)
        Else
            LabelCurrently.Text = "Not Target Pokemon - Resetting"
            Screenshot("NotTargetPokemon " & count)
        End If
    End Sub
    Sub ResetVariables()

        LabelCurrently.Refresh()
        XPos = GrassButtons.startx
        YPos = GrassButtons.starty
        LabelXPos.Text = "X Position: " & XPos
        LabelYPos.Text = "Y Position: " & YPos
        LabelTargetPatch.Text = "No Target Patch"
        ResetGrid()
        stepCount = 250
        LabelStepCount.Text = "Steps Until Repel: " & stepCount
        chainCount = 0
        LabelChainCount.Text = "Chain: " & chainCount
        stepsUntilReset = 0
        LabelStepsUntilReset.Text = "Steps Until Reset: " & stepsUntilReset
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        WhatPatchesShook()
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Are2ImagesDifferent(images(0), images(1))
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        wait(2)
        UseMaxRepel()
        'SoftReset()
    End Sub
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        wait(4.5)
        Movetest()
    End Sub
    Public Class gridy
        Inherits Label
        Public isSurrounded As Boolean = False
    End Class
    Sub CreateGrid()
        For i = 1 To GridSizeX
            For j = 1 To GridSizeY
                Dim Label1 As New gridy
                Controls.Add(Label1)


                Label1.Location = New Point(10 * i * 3, 10 + (10 * j * 3))
                Label1.Visible = True
                Label1.Width = 28
                Label1.Height = 28
                Label1.TextAlign = ContentAlignment.MiddleCenter

                Grid(i, j) = Label1

                If GrassButtons.ButtonGrid(i, j).BackColor = Color.LightGreen Or GrassButtons.ButtonGrid(i, j).BackColor = Color.Cyan Then
                    Grid(i, j).BackColor = Color.LightGreen
                    Label1.Text = ("-")
                End If
            Next
        Next
        Grid(XPos, YPos).Text = "P"
        CheckIfSurrounded()

    End Sub
    Sub CheckIfSurrounded()
        For i = 2 To GridSizeX - 1
            For j = 2 To GridSizeY - 1
                If Grid(i, j).BackColor = Color.LightGreen Then
                    If Grid(i + 1, j + 1).BackColor = Color.LightGreen And Grid(i - 1, j - 1).BackColor = Color.LightGreen And Grid(i, j + 1).BackColor = Color.LightGreen And Grid(i - 1, j + 1).BackColor = Color.LightGreen And Grid(i + 1, j - 1).BackColor = Color.LightGreen And Grid(i, j - 1).BackColor = Color.LightGreen And Grid(i + 1, j).BackColor = Color.LightGreen And Grid(i - 1, j).BackColor = Color.LightGreen And Grid(i, j - 1).BackColor = Color.LightGreen Then
                        Grid(i, j).isSurrounded = True
                    End If
                End If
            Next
        Next
    End Sub
    Sub ResetGrid()
        For i = 1 To GridSizeX
            For j = 1 To GridSizeY
                If Grid(i, j).Text = "X" Or Grid(i, j).Text = "P" Or Grid(i, j).Text = "S" Or Grid(i, j).Text = "F" Then
                    Grid(i, j).Text = "-"
                End If
            Next
        Next
        Grid(XPos, YPos).Text = "P"
    End Sub
    Sub Movetest()
        Do
            'Moove(72) 'H Left
            'Moove(75) 'K Right

            For i = 0 To 0
                Moove(85) 'U Up
            Next
            For i = 0 To 0
                Moove(74) 'J Down
            Next
        Loop
    End Sub
    Sub StoreGreens()
        For i = 0 To GIs.Length - 1
            GIs(i) = New Bitmap(fileLocationColours & "Green" & i + 1 & ".png")
            GPs(i) = GIs(i).GetPixel(1, 1)
        Next

        For i = 0 To SIs.Length - 1
            SIs(i) = New Bitmap(fileLocationColours & "ShinyColour" & i + 1 & ".png")
            SPs(i) = SIs(i).GetPixel(1, 1)
        Next
    End Sub
    Sub wait(time)
        HoldKeyDown(99, time)
    End Sub
    Private Sub HoldKeyDown(key As Byte, durationInSeconds As Decimal)
        Dim targetTime As DateTime = DateTime.Now().AddSeconds(durationInSeconds)
        keybd_event(key, MapVirtualKey(key, 0), 0, 0) ' Down
        While targetTime.Subtract(DateTime.Now()).TotalSeconds > 0
            Application.DoEvents()
        End While
        keybd_event(key, MapVirtualKey(key, 0), 2, 0) ' Up


    End Sub
    Private Declare Sub mouse_event Lib "user32.dll" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As IntPtr)
    Private Sub PerformMouseClick(ByVal LClick_RClick_DClick As String)
        Const MOUSEEVENTF_LEFTDOWN As Integer = 2
        Const MOUSEEVENTF_LEFTUP As Integer = 4
        Const MOUSEEVENTF_RIGHTDOWN As Integer = 6
        Const MOUSEEVENTF_RIGHTUP As Integer = 8
        If LClick_RClick_DClick = "RClick" Then
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero)
        ElseIf LClick_RClick_DClick = "LClick" Then
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            wait(0.1)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
        ElseIf LClick_RClick_DClick = "DClick" Then
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero)
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero)
        End If
    End Sub
    Private Declare Sub keybd_event Lib "user32.dll" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)

    <DllImport("User32.dll", SetLastError:=False, CallingConvention:=CallingConvention.StdCall,
       CharSet:=CharSet.Auto)>
    Public Shared Function MapVirtualKey(ByVal uCode As UInt32, ByVal uMapType As MappingType) As UInt32
    End Function

    Sub DeleteImages()
        For Each file In Directory.GetFiles(fileLocation)
            My.Computer.FileSystem.DeleteFile(file)
        Next
    End Sub

    Dim visited(GrassButtons.highestX, GrassButtons.highestY) As Boolean
    Dim previousX(GrassButtons.highestX, GrassButtons.highestY) As Integer
    Dim previousY(GrassButtons.highestX, GrassButtons.highestY) As Integer
    Dim dr() As Integer = {-1, 0, 1, 0}
    Dim dc() As Integer = {0, 1, 0, -1}
    Dim newX As Integer = XPos
    Dim newY As Integer = YPos
    Dim rq As New Queue()
    Dim cq As New Queue()
    Dim nodesLeftInLayer = 1
    Dim NodesInNextLayer = 0
    Sub SearchPath()

        wait(0.5)
        Dim sr As Integer = XPos
        Dim sc As Integer = YPos
        nodesLeftInLayer = 1
        NodesInNextLayer = 0
        Dim reachedEnd As Boolean = False
        Dim r As Integer = XPos
        Dim c As Integer = YPos
        Dim moveCount As Integer = 0
        rq.Clear()
        cq.Clear()
        rq.Enqueue(sr)
        cq.Enqueue(sc)
        visited(sr, sc) = True
        While rq.Count > 0 And cq.Count > 0 And reachedEnd = False

            r = rq.Dequeue()
            c = cq.Dequeue()
            If Grid(r, c).Text = "T" Then
                reachedEnd = True
                'break
            End If
            ExploreNeighbours(r, c)
            nodesLeftInLayer = nodesLeftInLayer - 1

            If nodesLeftInLayer = 0 Then
                nodesLeftInLayer = NodesInNextLayer
                NodesInNextLayer = 0
                moveCount = moveCount + 1

            End If

        End While


        Dim XmoveList As New List(Of Integer)
        Dim YmoveList As New List(Of Integer)
        For i = 0 To moveCount - 1
            XmoveList.Add(r)
            YmoveList.Add(c)
            Dim r2 As Integer = r
            Dim c2 As Integer = c
            r = previousX(r2, c2)
            c = previousY(r2, c2)
        Next

        XmoveList.Reverse()
        YmoveList.Reverse()
        For i = 0 To moveCount - 1

            Mover(XPos, YPos, XmoveList(i), YmoveList(i))
        Next
        ResetVisited()
        XmoveList.Clear()
        YmoveList.Clear()
    End Sub
    Sub ResetVisited()
        For i = 0 To GridSizeX
            For j = 0 To GridSizeY
                visited(i, j) = False
                previousX(i, j) = 0
                previousY(i, j) = 0
            Next
        Next
        rq.Clear()
        cq.Clear()

    End Sub
    Sub Mover(currentX, currentY, movetoX, movetoY)
        Dim key As Integer
        If currentX - movetoX = 1 Then
            key = 72 'hF
            Moove(key)

        ElseIf currentX - movetoX = -1 Then
            key = 75 'k
            Moove(key)
        ElseIf currentY - movetoY = 1 Then
            key = 85 'u
            Moove(key)

        ElseIf currentY - movetoY = -1 Then
            key = 74 'j
            Moove(key)
        End If
    End Sub
    Sub ExploreNeighbours(r, c)

        For i = 0 To 3
            newX = r + dr(i)
            newY = c + dc(i)
            If newX < 1 Or newY < 1 Or newX > GridSizeX Or newY > GridSizeY Then
                Continue For
            End If
            If visited(newX, newY) = True Then
                Continue For

            End If
            If Grid(newX, newY).Text = "X" Or Grid(newX, newY).Text = "F" Or Grid(newX, newY).BackColor <> Color.LightGreen Then
                Continue For
            End If
            previousX(newX, newY) = r
            previousY(newX, newY) = c
            rq.Enqueue(newX)
            cq.Enqueue(newY)
            visited(newX, newY) = True
            NodesInNextLayer = NodesInNextLayer + 1

        Next

    End Sub
    Function Same(lowX As Integer, highX As Integer, lowY As Integer, highY As Integer, image1 As Bitmap, image2 As Bitmap)
        Dim samenum As Integer = 0
        Dim diffnum As Integer = 0
        Dim nii As New Bitmap(image1.Width, image1.Height, Imaging.PixelFormat.Format32bppArgb)

        For x = 0 To nii.Width - 1
            For y = 0 To nii.Height - 1
                If x > lowX And x < highX Then
                    If y > lowY And y < highY Then
                        Dim pixelcolour As Color = image1.GetPixel(x, y)
                        Dim pixelcolour2 As Color = image2.GetPixel(x, y)

                        Dim a As Integer = pixelcolour.A
                        Dim b As Integer = pixelcolour.B
                        Dim r As Integer = pixelcolour.R
                        Dim g As Integer = pixelcolour.G
                        Dim a2 As Integer = pixelcolour2.A
                        Dim b2 As Integer = pixelcolour2.B
                        Dim r2 As Integer = pixelcolour2.R
                        Dim g2 As Integer = pixelcolour2.G

                        If SimilarColour(a, b, r, g, a2, b2, r, g) = True Then
                            samenum = samenum + 1
                        Else
                            diffnum = diffnum + 1
                            Return False
                        End If
                        'MsgBox(a & a2 & b & b2 & g & g2 & r & r2)
                    End If
                End If
            Next
        Next
        ' MsgBox(samenum)
        '  MsgBox(diffnum)


        If diffnum = 0 Then
            Return True
        Else
            Return False
        End If

    End Function
    Function SimilarColour(a As Integer, b As Integer, r As Integer, g As Integer, a2 As Integer, b2 As Integer, r2 As Integer, g2 As Integer)

        Dim variance As Integer = 7

        If a - variance < a2 And a2 < a + variance Then
            If b - variance < b2 And b2 < b + variance Then
                If r - variance < r2 And r2 < r + variance Then
                    If g - variance < g2 And g2 < g + variance Then
                        Return True
                    End If
                End If
            End If
        End If
        Return False
    End Function

    Function TargetFound()
        Dim found As Boolean = False
        LabelCurrently.Text = "Checking Target Pokemon"
        LabelCurrently.Refresh()

        Dim image2 As Bitmap = New Bitmap(fileLocation & TargetPokemon & " " & chainCount + 1 & ".png")
        Dim bmp As Bitmap
        Dim name As String

        bmp = Bitmap.FromFile(fileLocationPokemon & TargetPokemon & ".png")
        If Same(970, 1150, 220, 222, bmp, image2) = True Then
            foundPokemon = TargetPokemon
            foundPokemonImage = bmp
            found = True
        End If

        If found = False Then
            For Each file In Directory.GetFiles(fileLocationPokemon)
                name = My.Computer.FileSystem.GetName(file)
                name = name.Replace(".png", "")
                bmp = Bitmap.FromFile(fileLocationPokemon & name & ".png")
                If Same(970, 1150, 220, 222, bmp, image2) = True Then
                    foundPokemon = name
                    foundPokemonImage = bmp
                    found = True
                    Exit For
                End If
            Next
        End If
        If found = False Then
            foundPokemon = "unknown"
        End If

        If foundPokemon = TargetPokemon Then
            Return True
        Else
            Return False
        End If
    End Function

    Function IsShiny()
        If foundPokemon <> "unknown" Then
            Dim x As Integer
            Dim y As Integer
            Dim stringy As String = My.Computer.FileSystem.ReadAllText(fileLocationCheckArea & foundPokemon & ".txt")
            x = CInt(stringy(0) & stringy(1) & stringy(2))
            y = CInt(stringy(6) & stringy(7) & stringy(8))
            x = x + 1520
            y = y + 150
            Dim image2 As New Bitmap(fileLocation & TargetPokemon & " " & chainCount + 1 & ".png")

            'Screenshot("TestArea" & count)
            'Dim bmp As New Bitmap(fileLocation & "TestArea" & count & ".png")
            'bmp = CropBitmap(bmp, x, y, 10, 10)
            'bmp.Save(fileLocation & "TestArea " & count & ".png")
            'Dim bmp2 As New Bitmap(fileLocationPokemon & foundPokemon & ".png")
            'bmp2 = CropBitmap(bmp2, x, y, 10, 10)
            'bmp2.Save(fileLocation & "TestArea 2" & count & ".png")



            If Same(x, x + 2, y, y + 2, foundPokemonImage, image2) = True Then
                Return False
            Else
                Return True
            End If
        End If
        Return False
    End Function
    Sub WhatWasFound()
        Dim bool1 As Boolean = TargetFound()
        Dim bool2 As Boolean = IsShiny()

        If bool2 = True Then
            Screenshot("Full Odds Shiny")
            LabelFoundPokemon.Text = "Full Odds Shiny " & foundPokemon & " Found!"
            LabelFoundPokemon.Refresh()
            MsgBox("Full Odds Shiny Found!")
            End
        End If

        If foundPokemon = "unknown" Then
            LabelFoundPokemon.Text = "Unknown Pokemon Found"
            LabelFoundPokemon.Refresh()
        Else
            LabelFoundPokemon.Text = "Normal " & foundPokemon & " Found"
            LabelFoundPokemon.Refresh()
        End If

        If bool1 = False Then
            ChainBreak()
        End If

    End Sub
    Private Function CropBitmap(ByRef bmp As Bitmap, cropX As Integer, cropY As Integer, cropWidth As Integer, ByVal cropHeight As Integer) As Bitmap
        Dim rect As New Rectangle(cropX, cropY, cropWidth, cropHeight)
        Dim cropped As Bitmap = bmp.Clone(rect, bmp.PixelFormat)
        Return cropped
    End Function

    Private Sub ButtonClose_Click(sender As Object, e As EventArgs) Handles ButtonClose.Click
        Dim response As Integer
        response = MessageBox.Show("Are you sure you want to quit?", "Exit application?", MessageBoxButtons.YesNo)
        If response = vbYes Then
            Application.Exit()
            End
        Else
            Cursor.Position = New Point(900, 790)
            PerformMouseClick("LClick")
        End If

    End Sub
    Private Sub ButtonReset_Click(sender As Object, e As EventArgs)

        Dim response As Integer
        response = MessageBox.Show("Are you sure you want to reset?", "Reset?", MessageBoxButtons.YesNo)
        If response = vbYes Then
            Dim newForm = New Pokeradar

            Close()
            newForm.Show()
            Cursor.Position = New Point(900, 790)
        Else
            Cursor.Position = New Point(900, 790)
        End If

    End Sub
End Class