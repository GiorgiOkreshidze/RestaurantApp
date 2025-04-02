namespace Function.Models;

public enum ReservationStatus
{
    
    [System.ComponentModel.Description("Reserved")]
    Reserved,
    
    [System.ComponentModel.Description("In Progress")]
    InProgress,
    
    [System.ComponentModel.Description("Finished")]
    Finished,
    
    [System.ComponentModel.Description("Canceled")]
    Canceled,
}