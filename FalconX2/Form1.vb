﻿Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Windows.Automation
Imports Microsoft.VisualBasic.CompilerServices
Imports Transitions

Public Class Form1

    Dim SWP_NOSIZE As UInt32 = 1
    Dim SWP_NOMOVE As UInt32 = 2
    Dim SWP_NOZORDER As UInt32 = 4
    Dim SWP_NOREDRAW As UInt32 = 8
    Dim SWP_NOACTIVATE As UInt32 = 16
    Dim SWP_DRAWFRAME As UInt32 = 32
    Dim SWP_FRAMECHANGED As UInt32 = 32
    Dim SWP_SHOWWINDOW As UInt32 = 64
    Dim SWP_HIDEWINDOW As UInt32 = 128
    Dim SWP_NOCOPYBITS As UInt32 = 256
    Dim SWP_NOOWNERZORDER As UInt32 = 512
    Dim SWP_NOREPOSITION As UInt32 = 512
    Dim SWP_NOSENDCHANGING As UInt32 = 1024
    Dim SWP_DEFERERASE As UInt32 = 8192
    Dim SWP_ASYNCWINDOWPOS As UInt32 = 16384

    Dim WM_ACTIVATE = &H6

    Dim HWND_TOP As IntPtr = 0
    Dim HWND_BOTTOM As IntPtr = 1
    Dim HWND_TOPMOST As IntPtr = -1
    Dim HWND_NOTOPMOST As IntPtr = -2

    Enum ShowWindowCommands As Integer
        Hide = 0
        Normal = 1
        ShowMinimized = 2
        Maximize = 3
        ShowMaximized = 3
        ShowNoActivate = 4
        Show = 5
        Minimize = 6
        ShowMinNoActive = 7
        ShowNA = 8
        Restore = 9
        ShowDefault = 10
        ForceMinimize = 11
    End Enum

    Public Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure



    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll", ExactSpelling:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetParent(ByVal hWnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function SetParent(ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As UInt32) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function ShowWindow(hWnd As IntPtr, <MarshalAs(UnmanagedType.I4)> nCmdShow As ShowWindowCommands) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Private Declare Function SetProcessWorkingSetSize Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal dwMinimumWorkingSetSize As Int32, ByVal dwMaximumWorkingSetSize As Int32) As Int32
    Public Declare Function TerminateProcess Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal uExitCode As UInteger) As Integer

    Private TaskbarWidthFull As Integer
    Private notifyLeft As Integer
    Private tasklistPtr As IntPtr
    Private tasklistSWPtr As IntPtr
    Private trayWndPtr As IntPtr
    Private notifyPtr As IntPtr
    Private rebarPtr As IntPtr
    Private taskbarparent As IntPtr
    Private taskbarparentparent As IntPtr
    Private taskbarparentparentparent As IntPtr
    Private notifyparent As IntPtr
    Private traywndparent As IntPtr
    Private tasklistWidth As Integer
    Private tasklistHeight As Integer
    Private tasklistLeft As Integer
    Dim taskbarhori As Boolean

    Dim rct As RECT
    Dim refresh As Boolean = False
    Dim desktop As AutomationElement = AutomationElement.RootElement
    Dim condition As New OrCondition(New PropertyCondition(AutomationElement.ClassNameProperty, "Shell_TrayWnd"), New PropertyCondition(AutomationElement.ClassNameProperty, "Shell_TrayWnd"))
    Dim trayWnd As AutomationElement = desktop.FindFirst(TreeScope.Children, condition)
    Dim tasklist As AutomationElement = trayWnd.FindFirst(TreeScope.Descendants, New PropertyCondition(AutomationElement.ClassNameProperty, "MSTaskListWClass"))
    Dim notify As AutomationElement = trayWnd.FindFirst(TreeScope.Descendants, New PropertyCondition(AutomationElement.ClassNameProperty, "TrayNotifyWnd"))
    Dim startb As AutomationElement = trayWnd.FindFirst(TreeScope.Descendants, New PropertyCondition(AutomationElement.ClassNameProperty, "Start"))

    Private Sub start()

        NotifyIcon1.Text = "FalconX (Starting...)"

        '  Me.Hide()
        refresh = False

        Me.Left = 0
        Me.Width = Screen.PrimaryScreen.Bounds.Width

        Dim strx As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Microsoft\Windows\Start Menu\Programs\Startup"

        If File.Exists(strx + "\FalconX.lnk") Then
            RunAtStartupToolStripMenuItem.Checked = True

            If Application.StartupPath.Contains("40210ChrisAndriessen") Then
                Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
                objectValue = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
                Dim objectValue2 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, "SpecialFolders", New Object() {"Startup"}, Nothing, Nothing, Nothing))
                System.IO.File.WriteAllBytes(objectValue2 & "\FalconX.lnk", My.Resources.FalconX)
                RunAtStartupToolStripMenuItem.Checked = True
            Else
                Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
                objectValue = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
                Dim objectValue2 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, "SpecialFolders", New Object() {"Startup"}, Nothing, Nothing, Nothing))
                Dim objectValue3 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, "CreateShortcut", New Object() {Operators.ConcatenateObject(objectValue2, "\FalconX.lnk")}, Nothing, Nothing, Nothing))
                NewLateBinding.LateSet(objectValue3, Nothing, "TargetPath", New Object() {NewLateBinding.LateGet(objectValue, Nothing, "ExpandEnvironmentStrings", New Object() {Application.ExecutablePath}, Nothing, Nothing, Nothing)}, Nothing, Nothing)
                NewLateBinding.LateSet(objectValue3, Nothing, "WorkingDirectory", New Object() {NewLateBinding.LateGet(objectValue, Nothing, "ExpandEnvironmentStrings", New Object() {Application.StartupPath}, Nothing, Nothing, Nothing)}, Nothing, Nothing)
                NewLateBinding.LateSet(objectValue3, Nothing, "WindowStyle", New Object() {4}, Nothing, Nothing)
                NewLateBinding.LateCall(objectValue3, Nothing, "Save", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                RunAtStartupToolStripMenuItem.Checked = True
            End If

        Else
            RunAtStartupToolStripMenuItem.Checked = False
        End If

        If My.Settings.pos = Nothing Then
            ToolStripTextBox2.Text = 0
        Else
            ToolStripTextBox2.Text = Label3.Text
        End If

        AccelerationToolStripMenuItem.Checked = False
        NoneToolStripMenuItem.Checked = False
        CriticalDampingToolStripMenuItem.Checked = False
        DecelerationToolStripMenuItem.Checked = False
        EaseInEaseOutToolStripMenuItem.Checked = False
        LinearToolStripMenuItem.Checked = False

        If CheckBox1.Checked = True Then
            animation = 0
            NoneToolStripMenuItem.Checked = True
        End If

        If CheckBox2.Checked = True Then
            animation = 1
            AccelerationToolStripMenuItem.Checked = True
        End If

        If CheckBox3.Checked = True Then
            animation = 2
            CriticalDampingToolStripMenuItem.Checked = True
        End If

        If CheckBox4.Checked = True Then
            animation = 3
            DecelerationToolStripMenuItem.Checked = True
        End If

        If CheckBox5.Checked = True Then
            animation = 4
            EaseInEaseOutToolStripMenuItem.Checked = True
        End If

        If CheckBox6.Checked = True Then
            animation = 5
            LinearToolStripMenuItem.Checked = True
        End If

        '  Automation.AddAutomationEventHandler(WindowPattern.WindowOpenedEvent, desktop, TreeScope.Subtree, AddressOf ONUI)

        Dim t1 As System.Threading.Thread = New System.Threading.Thread(AddressOf ConstantlyCalculateWidth)
        t1.Start()

        System.Threading.Thread.Sleep(3000)

        Dim t3 As System.Threading.Thread = New System.Threading.Thread(AddressOf ConstantlyMoveTaskbar)
        t3.Start()

        SaveMemory()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Shown
        start()

    End Sub

    Sub ONUI(ByVal src As Object, ByVal e As AutomationEventArgs)
        Dim window As AutomationElement = TryCast(src, AutomationElement)
        Console.WriteLine(window.Current.Name)

        If window.Current.Name = "Windows Shell Experience Host" Then
            NotifyIcon1.Icon = My.Resources.icon_yellow_d6H_icon
            NotifyIcon1.Text = "FalconX (Restarting...)"

            refresh = True
            System.Threading.Thread.Sleep(5000)


            Application.Restart()

            Me.Dispose()
            End

        End If
    End Sub


  

    Private Sub ConstantlyCalculateWidth()
        Try

            tasklistPtr = tasklist.Current.NativeWindowHandle

            trayWndPtr = trayWnd.Current.NativeWindowHandle
            notifyPtr = notify.Current.NativeWindowHandle
            notifyLeft = notify.Current.BoundingRectangle.X
            tasklistWidth = Screen.PrimaryScreen.Bounds.Width
            tasklistHeight = trayWnd.Current.BoundingRectangle.Height

            notifyparent = GetParent(notifyPtr)
            taskbarparent = GetParent(tasklistPtr)
            traywndparent = GetParent(trayWndPtr)
            taskbarparentparent = GetParent(taskbarparent)
            taskbarparentparentparent = GetParent(taskbarparentparent)

            GetWindowRect(taskbarparentparent, rct)
            tasklistLeft = rct.Left

            ' Fixes Notification flickering
            SetParent(tasklist.Current.NativeWindowHandle, taskbarparent)



            'SetParent(trayWnd.Current.NativeWindowHandle, traywndparent)

            '  
            '   SetParent(notify.Current.NativeWindowHandle, traywndparent)
            '  SetParent(notify.Current.NativeWindowHandle, trayWndPtr)
            '   SetWindowPos(notify.Current.NativeWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)


            SetParent(tasklistPtr, trayWndPtr)




            SetParent(trayWndPtr, taskbarparentparent)

            SetParent(notifyparent, trayWndPtr)

            SetWindowPos(taskbarparentparent, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
            SetWindowPos(taskbarparentparent, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)

            SetWindowPos(tasklistPtr, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
            ' SetWindowPos(tasklistPtr, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)

            SetWindowPos(notifyPtr, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
            SetWindowPos(notifyPtr, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)

            SetWindowPos(notifyparent, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
            SetWindowPos(notifyparent, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)


            System.Threading.Thread.Sleep(500)

            Do
                Try

                    Dim TaskbarWidth As Integer = 0
                    Dim Laps As Integer

                    If refresh = True Then
                        Exit Sub
                    End If

                    Dim OldTaskbarCount As Integer
                    Dim TaskbarCount As Integer = 0
                    Dim tw As TreeWalker = TreeWalker.ControlViewWalker
                    Dim child As AutomationElement = tw.GetLastChild(tasklist)

                    tw = Nothing

                    TaskbarCount = child.Current.BoundingRectangle.Left

                    System.Threading.Thread.Sleep(400)

                    If Not TaskbarCount = OldTaskbarCount Then
                        OldTaskbarCount = TaskbarCount
                        For Each ui As AutomationElement In tasklist.FindAll(TreeScope.Descendants, New PropertyCondition(AutomationElement.IsControlElementProperty, True))
                            If Not ui.Current.Name = Nothing Then
                                TaskbarWidth = TaskbarWidth + ui.Current.BoundingRectangle.Width
                                System.Threading.Thread.Sleep(5)

                                If refresh = True Then
                                    Exit Sub
                                End If
                            End If
                        Next
                        TaskbarWidthFull = TaskbarWidth
                    End If

                    Laps = Laps + 1

                    If Laps = 10 Then
                        tasklistWidth = Screen.PrimaryScreen.Bounds.Width
                        '  tasklistHeight = Screen.PrimaryScreen.Bounds.Height
                        GetWindowRect(taskbarparentparent, rct)
                        tasklistLeft = rct.Left

                        '  If trayWnd.Current.BoundingRectangle.Height = Screen.PrimaryScreen.Bounds.Height Then
                        '  taskbarhori = False
                        'Else
                        ' taskbarhori = True
                        'End If

                    End If

                    If Laps = 20 Then
                        Laps = 0
                        SaveMemory()
                    End If
                Catch

                End Try
            Loop
        Catch ex As Exception
            NotifyIcon1.Icon = My.Resources.icon_yellow_d6H_icon
            NotifyIcon1.Text = "FalconX (Restarting...)"

            refresh = True
            System.Threading.Thread.Sleep(5000)

            Application.Restart()
        End Try

    End Sub

    Private Sub ConstantlyMoveTaskbar()

        System.Threading.Thread.Sleep(2000)

        NotifyIcon1.Icon = My.Resources.Icon1
        NotifyIcon1.Text = "FalconX"

        Do

            Try

                System.Threading.Thread.Sleep(10)

                If refresh = True Then
                    Exit Sub
                End If

                Dim TaskbarWidthHalf = TaskbarWidthFull / 2
                Dim Display1 As Integer = tasklistWidth / 2

                Dim Display1Vert As Integer = tasklistHeight / 2


                Dim dd As Integer

                If ToolStripTextBox2.Text = Nothing Then
                    dd = 0
                Else
                    dd = ToolStripTextBox2.Text
                End If

                Dim position = Display1 - TaskbarWidthHalf + dd - 2 'tasklistLeft - 2

                Dim positionVert = Display1Vert - TaskbarWidthHalf + dd - 2 'tasklistLeft - 2

                Gotoit = position

                Me.Invoke(New Action(Sub()
                                         Label1.Text = position
                                     End Sub))

                If cmoving = False Then
                    If refresh = True Then
                        Exit Sub
                    End If
                    '  SetWindowPos(taskbarparentparent, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
                    '  SetWindowPos(taskbarparentparent, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)


                    SetWindowPos(tasklistPtr, HWND_BOTTOM, position, 0, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_ASYNCWINDOWPOS Or SWP_NOSENDCHANGING Or SWP_NOACTIVATE Or SWP_NOCOPYBITS Or SWP_NOOWNERZORDER)

                    ' SetWindowPos(tasklistPtr, HWND_BOTTOM, 0, 0, tasklistWidth + 100, 100, SWP_NOZORDER Or SWP_NOMOVE Or SWP_ASYNCWINDOWPOS Or SWP_NOSENDCHANGING Or SWP_NOACTIVATE Or SWP_NOCOPYBITS Or SWP_NOOWNERZORDER)

                    '  SetWindowPos(notifyPtr, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE Or SWP_NOACTIVATE Or SWP_NOCOPYBITS)
                    SetWindowPos(notifyPtr, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE Or SWP_NOACTIVATE Or SWP_NOCOPYBITS)

                End If
            Catch

            End Try
        Loop
    End Sub



    Public Function SaveMemory() As Int32

        Return SetProcessWorkingSetSize(Diagnostics.Process.GetCurrentProcess.Handle, -1, -1)

    End Function

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        exitf()
    End Sub

    Private Sub exitf()
        Me.Invoke(New Action(Sub()

                                 NotifyIcon1.Icon = My.Resources.icon_red_MiX_icon
                                 NotifyIcon1.Text = "FalconX (Closing...)"

                                 My.Settings.Save()

                                 refresh = True

                                 System.Threading.Thread.Sleep(5000)

                                 restartexplorer()
                                 '  SetParent(trayWndPtr, traywndparent)
                                 ' SetParent(taskbarparent, taskbarparentparent)
                                 '  SetParent(tasklistPtr, taskbarparent)

                                 System.Threading.Thread.Sleep(1000)


                                 ' SetWindowPos(tasklistPtr, IntPtr.Zero, 0, 0, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_ASYNCWINDOWPOS Or SWP_NOSENDCHANGING Or SWP_NOACTIVATE)

                                 NotifyIcon1.Visible = False

                                 SaveMemory()
                                 Me.Dispose()

                                 Application.Exit()
                                 Me.Close()

                             End Sub))

    End Sub

    Private Sub Label1_TextChanged(sender As Object, e As EventArgs) Handles Label1.TextChanged
        SaveMemory()
        If refresh = True Then
            Exit Sub
        End If
        FluentMove()
    End Sub

    Sub FluentMove()
        Try

            cmoving = True

            If My.Settings.speedss = Nothing Then
                ToolStripTextBox1.Text = 500
            Else
                ToolStripTextBox1.Text = My.Settings.speedss
            End If

            If ToolStripTextBox1.Text = 0 Then
                ToolStripTextBox1.Text = 1
            End If

            Dim speed As Integer = ToolStripTextBox1.Text

            If animation = 1 Then
                Dim r1 As Transition = New Transition(New TransitionType_Acceleration(speed))
                r1.add(Panel1, "Left", Gotoit)
                r1.run()
            End If

            If animation = 2 Then
                Dim r3 As Transition = New Transition(New TransitionType_CriticalDamping(speed))
                r3.add(Panel1, "Left", Gotoit)
                r3.run()
            End If

            If animation = 3 Then
                Dim r4 As Transition = New Transition(New TransitionType_Deceleration(speed))
                r4.add(Panel1, "Left", Gotoit)
                r4.run()
            End If

            If animation = 4 Then
                Dim r5 As Transition = New Transition(New TransitionType_EaseInEaseOut(speed))
                r5.add(Panel1, "Left", Gotoit)
                r5.run()
            End If

            If animation = 5 Then
                Dim r7 As Transition = New Transition(New TransitionType_Linear(speed))
                r7.add(Panel1, "Left", Gotoit)
                r7.run()
            End If

            If animation = 0 Then
                Panel1.Left = Gotoit
            End If

            Do Until Panel1.Left = Gotoit
                Application.DoEvents()
            Loop
            cmoving = False

            SaveMemory()
        Catch ex As Exception

        End Try
    End Sub

    Dim Gotoit As Integer
    Dim cmoving As Boolean = False
    Dim animation As Integer = 2

    Private Sub Panel1_Move(sender As Object, e As EventArgs) Handles Panel1.Move
        ' If taskbarhori = True Then
        SetWindowPos(tasklistPtr, IntPtr.Zero, Panel1.Left, 0, 0, 0, SWP_NOZORDER Or SWP_NOSIZE Or SWP_ASYNCWINDOWPOS Or SWP_NOSENDCHANGING Or SWP_NOACTIVATE)
        ' End If

    End Sub

    Sub clearchecks()
        AccelerationToolStripMenuItem.Checked = False
        NoneToolStripMenuItem.Checked = False
        CriticalDampingToolStripMenuItem.Checked = False
        DecelerationToolStripMenuItem.Checked = False
        EaseInEaseOutToolStripMenuItem.Checked = False
        LinearToolStripMenuItem.Checked = False

        CheckBox1.Checked = False
        CheckBox2.Checked = False
        CheckBox3.Checked = False
        CheckBox4.Checked = False
        CheckBox5.Checked = False
        CheckBox6.Checked = False
    End Sub

    Private Sub NoneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NoneToolStripMenuItem.Click
        clearchecks()
        animation = 0
        NoneToolStripMenuItem.Checked = True
        CheckBox1.Checked = True
        My.Settings.Save()
    End Sub

    Private Sub AccelerationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AccelerationToolStripMenuItem.Click
        clearchecks()
        animation = 1
        AccelerationToolStripMenuItem.Checked = True
        CheckBox2.Checked = True
        My.Settings.Save()
    End Sub

    Private Sub CriticalDampingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CriticalDampingToolStripMenuItem.Click
        clearchecks()
        animation = 2
        CriticalDampingToolStripMenuItem.Checked = True
        CheckBox3.Checked = True
        My.Settings.Save()
    End Sub

    Private Sub DecelerationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DecelerationToolStripMenuItem.Click
        clearchecks()
        animation = 3
        DecelerationToolStripMenuItem.Checked = True
        CheckBox4.Checked = True
        My.Settings.Save()
    End Sub

    Private Sub EaseInEaseOutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EaseInEaseOutToolStripMenuItem.Click
        clearchecks()
        animation = 4
        EaseInEaseOutToolStripMenuItem.Checked = True
        CheckBox5.Checked = True
        My.Settings.Save()
    End Sub

    Private Sub LinearToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LinearToolStripMenuItem.Click
        clearchecks()
        animation = 5
        LinearToolStripMenuItem.Checked = True
        CheckBox6.Checked = True
        My.Settings.Save()
    End Sub

    Private Sub RunAtStartupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RunAtStartupToolStripMenuItem.Click

        Dim strx As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Microsoft\Windows\Start Menu\Programs\Startup"

        If File.Exists(strx + "\FalconX.lnk") Then
            Dim str As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\Microsoft\Windows\Start Menu\Programs\Startup"
            File.Delete(str + "\FalconX.lnk")
            RunAtStartupToolStripMenuItem.Checked = False
        Else
            If Application.StartupPath.Contains("40210ChrisAndriessen") Then
                Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
                objectValue = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
                Dim objectValue2 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, "SpecialFolders", New Object() {"Startup"}, Nothing, Nothing, Nothing))
                System.IO.File.WriteAllBytes(objectValue2 & "\FalconX.lnk", My.Resources.FalconX)
                RunAtStartupToolStripMenuItem.Checked = True
            Else
                Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
                objectValue = RuntimeHelpers.GetObjectValue(Interaction.CreateObject("WScript.Shell", ""))
                Dim objectValue2 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, "SpecialFolders", New Object() {"Startup"}, Nothing, Nothing, Nothing))
                Dim objectValue3 As Object = RuntimeHelpers.GetObjectValue(NewLateBinding.LateGet(objectValue, Nothing, "CreateShortcut", New Object() {Operators.ConcatenateObject(objectValue2, "\FalconX.lnk")}, Nothing, Nothing, Nothing))
                NewLateBinding.LateSet(objectValue3, Nothing, "TargetPath", New Object() {NewLateBinding.LateGet(objectValue, Nothing, "ExpandEnvironmentStrings", New Object() {Application.ExecutablePath}, Nothing, Nothing, Nothing)}, Nothing, Nothing)
                NewLateBinding.LateSet(objectValue3, Nothing, "WorkingDirectory", New Object() {NewLateBinding.LateGet(objectValue, Nothing, "ExpandEnvironmentStrings", New Object() {Application.StartupPath}, Nothing, Nothing, Nothing)}, Nothing, Nothing)
                NewLateBinding.LateSet(objectValue3, Nothing, "WindowStyle", New Object() {4}, Nothing, Nothing)
                NewLateBinding.LateCall(objectValue3, Nothing, "Save", New Object(-1) {}, Nothing, Nothing, Nothing, True)
                RunAtStartupToolStripMenuItem.Checked = True
            End If
        End If

    End Sub

    Private Sub restartexplorer()
        For Each MyProcess In Process.GetProcessesByName("explorer")
            MyProcess.Kill()
        Next

        Dim StartProcess As New Process
        Dim StartInfo As New ProcessStartInfo
        StartInfo.FileName = "explorer.exe"
        StartInfo.UseShellExecute = True
        StartProcess.Start(StartInfo)
    End Sub

    Private Sub ToolStripTextBox1_TextChanged(sender As Object, e As EventArgs) Handles ToolStripTextBox1.TextChanged
        Label2.Text = ToolStripTextBox1.Text
        My.Settings.Save()
    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        NotifyIcon1.Icon = My.Resources.icon_yellow_d6H_icon
        NotifyIcon1.Text = "FalconX (Restarting...)"

        refresh = True
        System.Threading.Thread.Sleep(5000)

        restartexplorer()

        System.Threading.Thread.Sleep(3000)


        Application.Restart()

        Me.Dispose()
        End
    End Sub

    Private Sub ToolStripTextBox2_TextChanged(sender As Object, e As EventArgs) Handles ToolStripTextBox2.TextChanged
        Label3.Text = ToolStripTextBox2.Text
        My.Settings.Save()
    End Sub

    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        AboutBox1.Show()
    End Sub
End Class