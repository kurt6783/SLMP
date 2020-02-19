Imports System.Net.Sockets
Public Class FX5U_SLMP
    Private Client As TcpClient
    Private Stream As NetworkStream
    Public Sub CreatConnect(ByVal IP As String, ByVal Port As Integer)
        Client = New TcpClient(IP, Port) '192.168.49.6 1025
        Stream = Client.GetStream
        Stream.ReadTimeout = 1000
        Stream.WriteTimeout = 1000
    End Sub
    Public Function Read(ByVal DM As Integer, ByVal Amount As Integer)
        Dim Head As String = "500000FFFF03000C00000001040000"
        Dim Type As String = "A8"   'DM = 00A8    M = 0090
        Dim Start As String = DM.ToString
        Start = Hex(DM)
        Select Case Start.Length
            Case 1
                Start = Start.PadLeft(2, "0").PadRight(4, "0") + "00"
            Case 2
                Start = Start.PadLeft(2, "0").PadRight(4, "0") + "00"
            Case 3
                Start = Mid(Start, 3, 2) + Mid(Start, 1, 1).PadLeft(0, "2") + "00"
            Case 4
                Start = Mid(Start, 3, 2) + Mid(Start, 1, 2) + "00"
        End Select
        Dim Count As String = Hex(Amount).ToString
        Count = Hex(Amount)
        Select Case Count.Length
            Case 1
                Count = Count.PadLeft(2, "0").PadRight(4, "0")
            Case 2
                Count = Count.PadLeft(2, "0").PadRight(4, "0")
            Case 3
                Count = Mid(Count, 3, 2) + Mid(Count, 1, 1).PadLeft(0, "2")
            Case 4
                Count = Mid(Count, 3, 2) + Mid(Count, 1, 2)
        End Select
        'Dim Command_String As String = Head + Start + Type + Count
        Dim Command_String As String = "01FF0A009001000020440B00"
        Dim Command_Byte(Command_String.Length / 2 - 1) As Byte
        For i = 0 To Command_String.Length / 2 - 1
            Command_Byte(i) = "&h" + Mid(Command_String, i * 2 + 1, 2)
        Next
        Stream.Write(Command_Byte, 0, Command_Byte.Count)
        Threading.Thread.Sleep(200)
        Dim Receive_Byte(Client.Available - 1) As Byte
        Stream.Read(Receive_Byte, 0, Client.Available)
        Dim Receive_String As String = Nothing
        For i = 0 To Amount - 1
            Receive_String = Receive_String + Hex(Receive_Byte(i * 2 + 12)).PadLeft(2, "0") + Hex(Receive_Byte(i * 2 + 11)).PadLeft(2, "0")
        Next
        Return Receive_String
    End Function
    Public Sub Write(ByVal DM As Integer, ByVal Data As String)
        Dim Head As String = "500000FFFF03000E00000001140000"
        Dim Type As String = "A8"
        Dim Start As String = Hex(DM).ToString
        Select Case Start.Length
            Case 1
                Start = Start.PadLeft(2, "0").PadRight(4, "0") + "00"
            Case 2
                Start = Start.PadLeft(2, "0").PadRight(4, "0") + "00"
            Case 3
                Start = Mid(Start, 3, 2) + Mid(Start, 1, 1).PadLeft(0, "2") + "00"
            Case 4
                Start = Mid(Start, 3, 2) + Mid(Start, 1, 2) + "00"
        End Select
        Dim Message As String = Data.ToString
        Select Case Message.Length
            Case 1
                Message = Message.PadLeft(2, "0").PadRight(4, "0")
            Case 2
                Message = Message.PadLeft(2, "0").PadRight(4, "0")
            Case 3
                Message = Mid(Message, 3, 2) + Mid(Message, 1, 1).PadLeft(0, "2")
            Case 4
                Message = Mid(Message, 3, 2) + Mid(Message, 1, 2)
        End Select
        Dim Command_String As String = Head + Start + Type + "0100" + Message
        Dim Command_Byte(Command_String.Length / 2 - 1) As Byte
        For i = 0 To Command_String.Length / 2 - 1
            Command_Byte(i) = "&h" + Mid(Command_String, i * 2 + 1, 2)
        Next
        Stream.Write(Command_Byte, 0, Command_Byte.Count)
        Threading.Thread.Sleep(50)
        Dim Receive_Byte(Client.Available - 1) As Byte
        Stream.Read(Receive_Byte, 0, Client.Available)
    End Sub
    Public Sub Dispose()
        Stream.Dispose()
        Stream = Nothing
        Client = Nothing
    End Sub
End Class
