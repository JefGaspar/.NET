namespace CA;

//omzetten naar byte zodat de enum klasse niet zo zwaar moet zijn,
//anders is het 32 bit(int), te veel voor enum
public enum EventCategory : byte
{
    sport = 1, //0 is default voor eerste element, zet op 1 gemakkelijker voor gebruiker
    music,
    theater,
    conference,
    workshop,
    networking,
    festival,
    webinair,
    fundraiser,
    tournament
    
}