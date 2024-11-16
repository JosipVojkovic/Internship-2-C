using System.Data.Common;
using System.Formats.Tar;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Channels;
using System.Transactions;

var users = new Dictionary<int, (string, string, string)>() {
    { 1, ("Josip", "Josipovic", "08.11.1998")},
    { 2, ("Marko", "Markovic", "20.12.1996")},
    { 3, ("Frane", "Franic", "15.05.1955")},
    { 4, ("Ivan", "Ivic", "19.07.1963")},
    { 5, ("Petra", "Petric", "03.01.1977")},
};

var usersAccounts = new Dictionary<int, Dictionary<string, decimal>>() {
    {1, new Dictionary<string, decimal>{
            { "ziro", 1254.59m},
            { "tekuci", 256.00m},
            { "prepaid", 100.00m}
        }
    },
    {2, new Dictionary<string, decimal>{
            { "ziro", 557.00m},
            { "tekuci", 984.31m},
            { "prepaid", 20.00m}
        }
    },
    {3, new Dictionary<string, decimal>{
            { "ziro", 5012.95m},
            { "tekuci", -50.00m},
            { "prepaid", 0.00m}
        }
    },
    {4, new Dictionary<string, decimal>{
            { "ziro", 2120.15m},
            { "tekuci", 82.44m},
            { "prepaid", 52.03m}
        }
    },
    {5, new Dictionary<string, decimal>{
            { "ziro", -100.00m},
            { "tekuci", 32.98m},
            { "prepaid", 07.23m}
        }
    },
};

var usersTransactions = new Dictionary<int, (int, string, string, decimal,  string, string, string)>() {
    {1, (1, "ziro", "prihod", 1000.00m, "standardna transakcija",  "placa", "15.10.2024 15:30:00") },
    {2, (1, "tekuci","rashod", 150.00m, "standardna transakcija",  "hrana", "22.10.2024 18:17:56") },
    {3, (1, "prepaid", "rashod",12.00m, "standardna transakcija", "prijevoz", "09.11.2024 19:32:22") },
    {4, (1,"tekuci", "prihod", 270.00m, "standardna transakcija",  "poklon", "11.09.2024 09:10:15") },
    {5, (1, "prepaid","rashod", 20.00m, "standardna transakcija",  "sport", "12.11.2024 22:45:12")},
    {6, (1, "tekuci", "rashod",32.99m, "standardna transakcija", "hrana", "13.11.2024 11:07:44")},
    {7, (1, "tekuci","rashod", 250.00m, "standardna transakcija",  "hrana", "24.10.2024 18:17:56") },
};

var format = "dd.MM.yyyy";
var formatWithHoursAndMinutes = "dd.MM.yyyy HH:mm:ss";

void wrongEntryy()
{
    Console.Clear();
    Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
    Console.WriteLine();
}

bool hasTwoDecimals(string number)
{
    if (number.Contains("."))
    {
        string[] parts = number.Split(".");
        return parts.Length == 2 && parts[1].Length == 2;
    }

    return false;
}

// Kreiranje korisnika
void createUser()
{
    
    Console.Write("Unesite ime novog korisnika: ");
    var name = Console.ReadLine();
    Console.Write("Unesite prezime novog korisnika: ");
    var lastname = Console.ReadLine();
    Console.Write("Unesite datum rodenja novog korisnika: ");
    var birthDate = Console.ReadLine();

    if(!DateTime.TryParseExact(birthDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None,out _))
    {
        wrongEntryy();
        createUser();
        return;
    }

    foreach (var user in users)
    {
        if(user.Value.Item1 == name && user.Value.Item2 == lastname)
        {
            wrongEntryy();
            createUser();
            return;
        }
    }
    
    usersTransactions.Add(usersTransactions.Last().Key + 1, (users.Last().Key + 1, "tekuci", "prihod", 100.00m, "kreiran racun", "poklon", DateTime.Now.ToString(formatWithHoursAndMinutes)));
    usersAccounts.Add(users.Last().Key + 1, new Dictionary<string, decimal>{
            { "ziro", 0.00m},
            { "tekuci", 100.00m},
            { "prepaid", 0.00m}
        }
    );
    users.Add(users.Last().Key + 1, (name?? "", lastname?? "", birthDate?? ""));

    Console.Clear();
    Console.WriteLine("Korisnik uspjesno kreiran:");
    Console.WriteLine($"{name} - {lastname} - {birthDate}");
    Console.WriteLine();
    Console.WriteLine("1 - Kreiraj novog korisnika");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    while (decision != "1" && decision != "0") {
        wrongEntryy();
        Console.WriteLine("1 - Kreiraj novog korisnika");
        Console.WriteLine("0 - Pocetni meni");
        Console.WriteLine();
        Console.Write("Odaberite radnju: ");
        decision = Console.ReadLine();
    }
    if (decision == "1")
    {
        Console.Clear();
        createUser();
    }
    else if (decision == "0")
    {
        Console.Clear();
        startingMenu();
    }
}

// Brisanje korisnika meni
void deleteUser() {
    
    Console.WriteLine("1 - Po id-u");
    Console.WriteLine("2 - Po imenu i prezimenu");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            deleteById();
            return;
        case "2":
            Console.Clear();
            deleteByName();
            return;
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            deleteUser();
            return;
    }
}

// Brisanje korisnika prema id-u
void deleteById()
{
    Console.Write("Unesite id korisnika kojeg zelite obrisati: ");
    var id = Console.ReadLine();

    foreach (var user in users)
    {
        if (id != null && user.Key == int.Parse(id))
        {
            users.Remove(int.Parse(id));
            foreach(var transaction in usersTransactions)
            {
                if(transaction.Value.Item1 == int.Parse(id))
                {
                    usersTransactions.Remove(transaction.Key);
                }
                
            }

            Console.Clear();
            Console.WriteLine($"Korisnik id-{id} uspjesno obrisan.");
            Console.WriteLine();
            Console.WriteLine("1 - Obrisite novog korisnika");
            Console.WriteLine("0 - Pocetni meni");
            Console.WriteLine();
            Console.Write("Odaberite radnju: ");
            var decision = Console.ReadLine();
            while (decision != "0" && decision != "1" )
            {
                wrongEntryy();
                Console.WriteLine("1 - Obrisite novog korisnika");
                Console.WriteLine("0 - Pocetni meni");
                Console.WriteLine();
                Console.Write("Odaberite radnju: ");
                decision = Console.ReadLine();
            }

            switch (decision)
            {
                case "1":
                    Console.Clear();
                    deleteById();
                    return;
                case "0":
                    Console.Clear();
                    startingMenu();
                    return;
            }
        }
     }
    wrongEntryy();
    deleteById();
}

// Brisanje korisnika prema imenu i prezimenu
void deleteByName()
{
    Console.Write("Unesite ime korisnika kojeg zelite obrisati: ");
    var name = Console.ReadLine();
    Console.Write("Unesite prezime korisnika kojeg zelite obrisati: ");
    var lastname = Console.ReadLine();

    foreach (var user in users)
    {
        if (user.Value.Item1 == name && user.Value.Item2 == lastname)
        {
            users.Remove(user.Key);
            foreach (var transaction in usersTransactions)
            {
                if (transaction.Value.Item1 == user.Key)
                {
                    usersTransactions.Remove(transaction.Key);
                }

            }
            Console.Clear();
            Console.WriteLine($"Korisnik {name} {lastname} uspjesno obrisan.");
            Console.WriteLine();
            Console.WriteLine("1 - Obrisite novog korisnika");
            Console.WriteLine("0 - Pocetni meni");
            Console.WriteLine();
            Console.Write("Odaberite radnju: ");
            var decision = Console.ReadLine();

            while (decision != "0" && decision != "1")
            {
                wrongEntryy();
                Console.WriteLine("1 - Obrisite novog korisnika");
                Console.WriteLine("0 - Pocetni meni");
                Console.WriteLine();
                Console.Write("Odaberite radnju: ");
                decision = Console.ReadLine();
            }

            switch (decision)
            {
                case "1":
                    Console.Clear();
                    deleteByName();
                    return;
                case "0":
                    Console.Clear();
                    startingMenu();
                    return;
            }
        }
    }
    wrongEntryy();
    deleteByName(); 
}

// Uredivanje korisnika
void updateUser()
{
    Console.Write("Unesite id korisnika cije podatke zelite urediti: ");
    var id = Console.ReadLine();
    Console.Clear();

    foreach (var user in users)
    {
        if (id != null && user.Key == int.Parse(id))
        {
            Console.WriteLine("Trenutni podatci korisnika: ");
            Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3}");
            Console.WriteLine();
            Console.Write("Upisite novo ime: ");
            var name = Console.ReadLine();
            Console.Write("Upisite novo prezime: ");
            var lastname = Console.ReadLine();
            Console.Write("Upisite novi datum rodenja: ");
            var birthDate = Console.ReadLine();

            if (!DateTime.TryParseExact(birthDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                wrongEntryy();
                updateUser();
            }
            else 
            {
                users[user.Key] = (name, lastname, birthDate);
            }

            Console.Clear();
            Console.WriteLine($"Korisnik id-{id} uspjesno ureden.");
            Console.WriteLine();
            Console.WriteLine("1 - Uredite novog korisnika");
            Console.WriteLine("0 - Pocetni meni");
            Console.WriteLine();
            Console.Write("Odaberite radnju: ");
            var decision = Console.ReadLine();

            while (decision != "0" && decision != "1")
            {
                wrongEntryy();
                Console.WriteLine("1 - Uredite novog korisnika");
                Console.WriteLine("0 - Pocetni meni");
                Console.WriteLine();
                Console.Write("Odaberite radnju: ");
                decision = Console.ReadLine();
            }

            switch (decision)
            {
                case "1":
                    Console.Clear();
                    updateUser();
                    return;
                case "0":
                    Console.Clear();
                    startingMenu();
                    return;
            }
            
        }
    }

    updateUser();
}

// Pregledavanje korisnika koji imaju barem jedan racun u minusu
void reviewUsersByAccount()
{
    Console.WriteLine("Korisnici koji imaju barem jedan racun u minusu:");
    Console.WriteLine();

    var count = 1;
    while (count <= users.Count)
    {
        if (usersAccounts.ContainsKey(count))
        {
            foreach (var account in usersAccounts[count])
            {
                if (account.Value < 0.00m)
                {
                    Console.WriteLine($"{count} - {users[count].Item1} - {users[count].Item2} - {users[count].Item3}");
                    count++;
                }
            }
        }
        count++;
    }
    Console.WriteLine();
    Console.Write("Unesite 0 za povratak na pocetni meni: ");
    var decision = Console.ReadLine() ;

    switch (decision) 
    {
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            reviewUsersByAccount();
            return;

    }

}

// Pregledavanje korisnika koji su stariji od 30 godina
void reviewUsersByAge() 
{
    Console.WriteLine("Korisnici stariji od 30 godina:");
    Console.WriteLine();

    foreach (var user in users)
    {
        if (DateTime.Parse(user.Value.Item3) < DateTime.Now.AddYears(-30)) 
        {
            Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3}");
        }
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak na pocetni meni: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            reviewUsersByAge();
            return;
    }
}

// Pregledavanje korisnika abecedno po prezimenu
void reviewUsersAlphabetically()
{
    Console.Clear();
    var goBack = false;
    while (!goBack)
    {

        Console.WriteLine("Korisnici poredani abecedno prema prezimenu:");
        Console.WriteLine();

        foreach (var user in users.OrderBy(pair => pair.Value.Item2))
        {
            Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3}");
        }

        Console.WriteLine();
        Console.Write("Unesite 0 za povratak na pocetni meni: ");
        var decision = Console.ReadLine();

        if (decision == "0")
        {
            goBack = true;
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Pogresan unos. Pokusaj ponovno.");
            Console.WriteLine();
        }
    }

    Console.Clear();
    startingMenu();
    return;
}

// Pregledavanje korisnika meni
void reviewUsers()
{
    Console.WriteLine("1 - Ispis korisnika abecedno po prezimenu");
    Console.WriteLine("2 - Ispis korisnika koji imaju vise od 30 godina");
    Console.WriteLine("3 - Korisnici sa racunom u minusu");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            reviewUsersAlphabetically();
            return;
        case "2": 
            Console.Clear();
            reviewUsersByAge();
            return;
        case "3":
            Console.Clear();
            reviewUsersByAccount();
            return;
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            reviewUsers();
            return;
    }
}

// Korisnici meni
void usersMenu()
{
    Console.WriteLine("1 - Unos novog korisnika");
    Console.WriteLine("2 - Brisanje korisnika");
    Console.WriteLine("3 - Uredivanje korisnika");
    Console.WriteLine("4 - Pregled korisnika");
    Console.WriteLine("0 - Pocetni meni");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            createUser();
            return;
        case "2":
            Console.Clear();
            deleteUser();
            return;
        case "3":
            Console.Clear();
            updateUser();
            return;
        case "4":
            Console.Clear();
            reviewUsers();
            return;
        case "0":
            Console.Clear();
            startingMenu(); // natrag na pocetni meni
            return;
        default:
            wrongEntryy();
            usersMenu();
            return;
    }
}
void createTransaction(int userId, string selectedAccount)
{
    var entry = "";
    Console.Clear();

    while (!decimal.TryParse(entry, out _) || decimal.Parse(entry, CultureInfo.InvariantCulture) <= 0) {
        Console.Write("Unesite iznos transakcije u decimalnom obliku (npr. 9.99): ");
        entry = Console.ReadLine();

        if (!decimal.TryParse(entry, out _) || decimal.Parse(entry, CultureInfo.InvariantCulture) <= 0)
        {
            wrongEntryy();
        }
    }

    decimal transactionAmount = Decimal.Round(decimal.Parse(entry, CultureInfo.InvariantCulture), 2);
    
    Console.Clear();
    Console.WriteLine("Unesite opis transakcije ili ostavite prazno za zadani opis 'standardna transakcija': ");
    var transactionDescription = Console.ReadLine();

    transactionDescription = transactionDescription == "" ? "standardna transakcija" : transactionDescription;

    var transactionType = "";
    Console.Clear();

    while (transactionType != "1" && transactionType != "2")
    {
        Console.WriteLine("Tip transakcije: ");
        Console.WriteLine();
        Console.WriteLine("1 - Prihod");
        Console.WriteLine("2 - Rashod");
        Console.WriteLine();
        Console.Write("Odaberite tip: ");
        transactionType = Console.ReadLine();

        if (transactionType != "1" && transactionType != "2")
        {
            wrongEntryy();
        }

    }

    var transactionCategory = "";
    Console.Clear();

    while (transactionCategory != "1" && transactionCategory != "2" && transactionCategory != "3")
    {
        Console.WriteLine("Kategorije:");
        Console.WriteLine();

        switch (transactionType) 
        {
            case "1":
                Console.WriteLine("1 - Placa");
                Console.WriteLine("2 - Honorar");
                Console.WriteLine("3 - Poklon");
                break;
            case "2": 
                Console.WriteLine("1 - Hrana");
                Console.WriteLine("2 - Prijevoz");
                Console.WriteLine("3 - Sport");
                break;
        }
    
        Console.WriteLine();
        Console.Write("Odaberite kategoriju: ");
        transactionCategory = Console.ReadLine();

        if (transactionCategory != "1" && transactionCategory != "2" && transactionCategory != "3")
        {
            wrongEntryy();
        }
    }

    var transactionDate = "";
    Console.Clear();

    while (!DateTime.TryParseExact(transactionDate, formatWithHoursAndMinutes, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) 
    {
        Console.WriteLine("Unesite datum transakcije ili ostavite prazno za trenutni datum: ");
        Console.WriteLine("Napomena! Uneseni datum mora biti u ovom formatu: dd.MM.yyyy HH:mm:ss (npr. 14.11.2024 15:30:25)");
        transactionDate = Console.ReadLine();

        if (transactionDate == "")
        {
            transactionDate = DateTime.Now.ToString(formatWithHoursAndMinutes);
            
        }
        if (!DateTime.TryParseExact(transactionDate, formatWithHoursAndMinutes, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            wrongEntryy();
        }
    }

    var type = "";
    var category = "";

    switch (transactionType) 
    {
        case "1":
            type = "prihod";
            switch (transactionCategory) 
            {
                case "1":
                    category = "placa";
                    break;
                case "2":
                    category = "honorar";
                    break;
                case "3":
                    category = "poklon";
                    break;
            }
            break;
        case "2":
            type = "rashod";
            switch (transactionCategory)
            {
                case "1":
                    category = "hrana";
                    break;
                case "2":
                    category = "prijevoz";
                    break;
                case "3":
                    category = "sport";
                    break;
            }
            break;
    }

    usersTransactions.Add(usersTransactions.Last().Key + 1, (userId, selectedAccount, type, transactionAmount, transactionDescription,  category, transactionDate));

    Console.Clear();
    Console.WriteLine("Transakcija uspjesno kreirana:");
    Console.WriteLine();
    Console.WriteLine("Tip - Iznos(eur) - Opis - Kategorija - Datum");
    Console.WriteLine($"{type} - {transactionAmount} - {transactionDescription} - {category} - {transactionDate}");
    Console.WriteLine();

    account(userId);
    return;
}

void deleteTransactionById(int userId, string selectedAccount)
{
    Console.Write("Unesite id transakcije koju zelite obrisati ili 0 za povratak: ");
    var id = Console.ReadLine();
    int idInteger;

    if (id == "0")
    {
        Console.Clear();
        deleteTransactions(userId, selectedAccount);
        return;
    }
    else if (int.TryParse(id, out idInteger))
    {
        if (usersTransactions.ContainsKey(idInteger) && usersTransactions[idInteger].Item1 == userId && usersTransactions[idInteger].Item2 == selectedAccount)
        {
            usersTransactions.Remove(idInteger);
            Console.Clear();
            Console.WriteLine($"Transakcija id-{idInteger} uspjesno obrisana.");
            Console.WriteLine();
            account(userId);
            return;
        }
        else
        {
            Console.Clear();
            Console.WriteLine($"Ne postoji transakcija sa id-{idInteger} za ovaj racun. Pokusajte ponovno.");
            Console.WriteLine();
            deleteTransactionById(userId, selectedAccount);
            return;
        }
    }

    wrongEntryy();
    deleteTransactionById(userId, selectedAccount);
    return;
}

void deleteTransactionsLessThen(int userId, string selectedAccount)
{
    Console.Write("Unesite iznos u decimalnom obliku (npr. 10.99) za koji ce se sve transakcije sa iznosom manjim od tog izbrisati ili 0 za povratak: ");
    var amount = Console.ReadLine();
    decimal amountInteger;

    if (amount == "0")
    {
        Console.Clear();
        deleteTransactions(userId, selectedAccount);
        return;
    }
    else if (!hasTwoDecimals(amount) || !decimal.TryParse(amount, NumberStyles.Number, CultureInfo.InvariantCulture, out amountInteger))
    {
        wrongEntryy();
        deleteTransactionsLessThen(userId, selectedAccount);
        return;
    }
    else if (!usersTransactions.Values.Any(transaction => transaction.Item1 == userId && transaction.Item2 == selectedAccount && transaction.Item4 < amountInteger))
    {
        Console.Clear();
        Console.WriteLine("Ne postoji trazena transakcija za ovaj racun...");
        Console.WriteLine();
        deleteTransactions(userId, selectedAccount);
        return;
    }
    else
    {   
        foreach (var transaction in usersTransactions)
        {
            if (transaction.Value.Item1 == userId && transaction.Value.Item2 == selectedAccount && transaction.Value.Item4 < amountInteger)
            {
                usersTransactions.Remove(transaction.Key);
            }
        }
        Console.Clear();
        Console.WriteLine("Transakcije uspjesno obrisane!");
        Console.WriteLine();
        transactionsMenu(userId, selectedAccount);
        return;
    }
}

void deleteTransactionsMoreThen(int userId, string selectedAccount)
{
    Console.Write("Unesite iznos u decimalnom obliku (npr. 10.99) za koji ce se sve transakcije sa iznosom vecim od tog izbrisati ili 0 za povratak: ");
    var amount = Console.ReadLine();
    decimal amountInteger;

    if (amount == "0")
    {
        Console.Clear();
        deleteTransactions(userId, selectedAccount);
        return;
    }
    else if (!hasTwoDecimals(amount) || !decimal.TryParse(amount, NumberStyles.Number, CultureInfo.InvariantCulture, out amountInteger))
    {
        wrongEntryy();
        deleteTransactionsLessThen(userId, selectedAccount);
        return;
    }
    else if (!usersTransactions.Values.Any(transaction => transaction.Item1 == userId && transaction.Item2 == selectedAccount && transaction.Item4 > amountInteger))
    {
        Console.Clear();
        Console.WriteLine("Ne postoji trazena transakcija za ovaj racun...");
        Console.WriteLine();
        deleteTransactions(userId, selectedAccount);
        return;
    }
    else
    {
        foreach (var transaction in usersTransactions)
        {
            if (transaction.Value.Item1 == userId && transaction.Value.Item2 == selectedAccount && transaction.Value.Item4 > amountInteger)
            {
                usersTransactions.Remove(transaction.Key);
            }
        }
        Console.Clear();
        Console.WriteLine("Transakcije uspjesno obrisane!");
        Console.WriteLine();
        transactionsMenu(userId, selectedAccount);
        return;
    }
}

void deleteTransactionsType(int userId, string selectedAccount, string type)
{
    if (!usersTransactions.Values.Any(transaction => transaction.Item1 == userId && transaction.Item2 == selectedAccount && transaction.Item3 == type))
    {
        Console.Clear();
        Console.WriteLine("Ne postoji trazena transakcija za ovaj racun...");
        Console.WriteLine();
        deleteTransactions(userId, selectedAccount);
        return;
    }

    foreach (var transaction in usersTransactions)
    {
        if(transaction.Value.Item1 == userId && transaction.Value.Item2 == selectedAccount && transaction.Value.Item3 == type)
        {
            usersTransactions.Remove(transaction.Key);
        }
    }

    Console.Clear();
    Console.WriteLine("Transakcije uspjesno obrisane!");
    Console.WriteLine();
    transactionsMenu(userId, selectedAccount);
    return;
}

void deleteTransactionsByCategory(int userId, string selectedAccount) 
{
    Console.WriteLine("Kategorije:");
    Console.WriteLine();
    Console.WriteLine("1 - Placa");
    Console.WriteLine("2 - Honorar");
    Console.WriteLine("3 - Poklon");
    Console.WriteLine("4 - Hrana");
    Console.WriteLine("5 - Prijevoz");
    Console.WriteLine("6 - Sport");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var selectedNumber = Console.ReadLine();
    var category = "";

    switch (selectedNumber)
    {
        case "1":
            category = "placa";
            break;
        case "2":
            category = "honorar";
            break;
        case "3":
            category = "poklon";
            break;
        case "4":
            category = "hrana";
            break;
        case "5":
            category = "prijevoz";
            break;
        case "6":
            category = "sport";
            break;
        default:
            wrongEntryy();
            deleteTransactionsByCategory(userId, selectedAccount);
            return;
    }

    if (!usersTransactions.Values.Any(transaction => transaction.Item1 == userId && transaction.Item2 == selectedAccount && transaction.Item6 == category))
    {
        Console.Clear();
        Console.WriteLine("Ne postoji trazena transakcija za ovaj racun...");
        Console.WriteLine();
        deleteTransactions(userId, selectedAccount);
        return;
    }

    foreach (var transaction in usersTransactions)
    {
        if (transaction.Value.Item1 == userId && transaction.Value.Item2 == selectedAccount && transaction.Value.Item6 == category)
        {
            usersTransactions.Remove(transaction.Key);
        }
    }

    Console.Clear();
    Console.WriteLine("Transakcije uspjesno obrisane!");
    Console.WriteLine();
    transactionsMenu(userId, selectedAccount);
    return;
}

void deleteTransactions(int userId, string selectedAccount) 
{
    Console.WriteLine($"{users[userId].Item1} {users[userId].Item2} => {selectedAccount} => brisanje transakcije");
    Console.WriteLine();
    Console.WriteLine("1 - Po id-u");
    Console.WriteLine("2 - Ispod unesenog iznosa");
    Console.WriteLine("3 - Iznad unesenog iznosa");
    Console.WriteLine("4 - Svih prihoda");
    Console.WriteLine("5 - Svih rashoda");
    Console.WriteLine("6 - Za odabranu kategoriju");
    Console.WriteLine("7 - Natrag");
    Console.WriteLine("0 - Pocetni meni");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            deleteTransactionById(userId, selectedAccount);
            return;
        case "2":
            Console.Clear();
            deleteTransactionsLessThen(userId, selectedAccount);
            return;
        case "3":
            Console.Clear();
            deleteTransactionsMoreThen(userId, selectedAccount);
            return;
        case "4":
            Console.Clear();
            deleteTransactionsType(userId, selectedAccount, "prihod");
            return;
        case "5":
            Console.Clear();
            deleteTransactionsType(userId, selectedAccount, "rashod");
            return;
        case "6":
            Console.Clear();
            deleteTransactionsByCategory(userId, selectedAccount);
            return;
        case "7":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            deleteTransactions(userId, selectedAccount);
            return;
    }
}

void updateTransaction(int userId, string selectedAccount)
{
    Console.Write("Unesite id transakcije koju zelite urediti: ");
    var id = Console.ReadLine();

    if (id != null && usersTransactions.ContainsKey(int.Parse(id)) && usersTransactions[int.Parse(id)].Item1 == userId && usersTransactions[int.Parse(id)].Item2 == selectedAccount)
    {
        var transactionType = "";
        Console.Clear();

        while(transactionType != "1" && transactionType != "2")
        {
            Console.WriteLine("1 - Prihod");
            Console.WriteLine("2 - Rashod");
            Console.WriteLine();
            Console.Write("Odaberite novi tip transakcije: ");
            transactionType = Console.ReadLine();

            if (transactionType != "1" && transactionType != "2")
            {
                wrongEntryy();
            }
        }

        transactionType = transactionType == "1" ? "prihod" : "rashod";

        Console.Clear();
        var amount = "";
        decimal transactionAmount;

        while(!decimal.TryParse(amount, NumberStyles.Number, CultureInfo.InvariantCulture, out transactionAmount) || decimal.Parse(amount) <= 0)
        {
            Console.Write("Unesite novi iznos transakcije, mora biti u decimalnom obliku (npr. 10.99): ");
            amount = Console.ReadLine();

            if (!decimal.TryParse(amount, NumberStyles.Number, CultureInfo.InvariantCulture, out _) || decimal.Parse(amount) <= 0)
            {
                wrongEntryy();
            }
        }

        Console.Clear();
        Console.Write("Unesite novi opis transakcije ili ostavite prazno za zadani opis 'standarnda transakcija': ");
        var transactionDescription = Console.ReadLine();

        if (transactionDescription == "")
        {
            transactionDescription = "standardna transakcija";
        }

        Console.Clear();
        var transactionCategory = "";

        while (transactionCategory != "1" && transactionCategory != "2" && transactionCategory != "3")
        {   
            Console.WriteLine(transactionType == "prihod"? "1 - Placa": "1 - Hrana");
            Console.WriteLine(transactionType == "prihod"? "2 - Honorar": "2 - Prijevoz");
            Console.WriteLine(transactionType == "prihod" ? "3 - Poklon" : "3 - Sport");
            Console.WriteLine();
            Console.Write("Odaberite novu kategoriju: ");
            transactionCategory = Console.ReadLine();

            if (transactionCategory != "1" && transactionCategory != "2" && transactionCategory != "3")
            {
                wrongEntryy();
            }
        }

        switch(transactionCategory)
        {
            case "1":
                transactionCategory = transactionType == "prihod" ? "placa" : "hrana";
                break;
            case "2":
                transactionCategory = transactionType == "prihod" ? "honorar" : "prijevoz";
                break;
            case "3":
                transactionCategory = transactionType == "prihod" ? "poklon" : "sport";
                break;
        }

        var transactionDate = "";
        Console.Clear();

        while (!DateTime.TryParseExact(transactionDate, formatWithHoursAndMinutes, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            Console.WriteLine("Unesite novi datum transakcije ili ostavite prazno za trenutni datum: ");
            Console.WriteLine("Napomena! Uneseni datum mora biti u ovom formatu: dd.MM.yyyy HH:mm:ss (npr. 14.11.2024 15:30:25)");
            transactionDate = Console.ReadLine();

            if (transactionDate == "")
            {
                transactionDate = DateTime.Now.ToString(formatWithHoursAndMinutes);

            }
            if (!DateTime.TryParseExact(transactionDate, formatWithHoursAndMinutes, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                wrongEntryy();
            }
        }

        if (transactionType != null && transactionDescription != null && transactionCategory != null && transactionDate != null)
        {
            usersTransactions[int.Parse(id)] = (userId, selectedAccount, transactionType, transactionAmount, transactionDescription, transactionCategory, transactionDate);
        }

        Console.Clear();
        Console.WriteLine($"Transakcija id-{id} uspjesno uredena.");
        Console.WriteLine();
        transactionsMenu(userId, selectedAccount);
        return;
    }

    wrongEntryy();
    updateTransaction(userId, selectedAccount);
    return;
}

void allTransactions(int userId, string selectedAccount)
{
    Console.WriteLine("Sve transakcije:");
    Console.WriteLine();

    if(!usersTransactions.Values.Any(transaction => transaction.Item1 == userId && transaction.Item2 == selectedAccount))
    {
        Console.WriteLine("Ne postoji transakcija za ovaj racun...");
    }else
    {   
        Console.WriteLine("Tip - Iznos(eur) - Opis - Kategorija - Datum");
        Console.WriteLine();
        foreach (var transaction in usersTransactions)
        {
            if (transaction.Value.Item1 == userId && transaction.Value.Item2 == selectedAccount)
            {
                Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
            }
        }
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch(decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allTransactions(userId, selectedAccount);
            return;
    }
}

void allTransactionsByAmountUp(int userId, string selectedAccount)
{
    Console.WriteLine("Sve transakcije sortirane prema iznosu uzlazno:");
    Console.WriteLine();
    var sortedFilteredTransactions = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount)
            .OrderBy(user => user.Value.Item4)
            .ToDictionary(user => user.Key, user => user.Value);

    Console.WriteLine($"Tip - Iznos(eur) - Opis - Kategorija - Datum");
    Console.WriteLine();

    foreach (var transaction in sortedFilteredTransactions)
    {
        
        Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu (userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allTransactionsByAmountUp(userId, selectedAccount);
            break;
    }

}

void allTransactionsByAmountDown(int userId, string selectedAccount)
{
    Console.WriteLine("Sve transakcije sortirane prema iznosu silazno:");
    Console.WriteLine();
    var sortedFilteredTransactions = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount)
            .OrderByDescending(user => user.Value.Item4)
            .ToDictionary(user => user.Key, user => user.Value);

    Console.WriteLine($"Tip - Iznos(eur) - Opis - Kategorija - Datum");
    Console.WriteLine();

    foreach (var transaction in sortedFilteredTransactions)
    {

        Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allTransactionsByAmountDown(userId, selectedAccount);
            break;
    }

}

void allTransactionsByDescription(int userId, string selectedAccount)
{
    Console.WriteLine("Sve transakcije sortirane abecedno prema opisu:");
    Console.WriteLine();
    var sortedFilteredTransactions = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount)
            .OrderBy(user => user.Value.Item5)
            .ToDictionary(user => user.Key, user => user.Value);

    Console.WriteLine($"Tip - Iznos(eur) - Opis - Kategorija - Datum");
    Console.WriteLine();

    foreach (var transaction in sortedFilteredTransactions)
    {

        Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allTransactionsByDescription(userId, selectedAccount);
            break;
    }

}

void allTransactionsByDateUp(int userId, string selectedAccount)
{
    Console.WriteLine("Sve transakcije sortirane prema datumu uzlazno:");
    Console.WriteLine();
    var sortedFilteredTransactions = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount)
            .OrderBy(user => DateTime.ParseExact(user.Value.Item7, formatWithHoursAndMinutes, null))
            .ToDictionary(user => user.Key, user => user.Value);

    Console.WriteLine($"Tip - Iznos(eur) - Opis - Kategorija - Datum");
    Console.WriteLine();

    foreach (var transaction in sortedFilteredTransactions)
    {

        Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allTransactionsByDateUp(userId, selectedAccount);
            break;
    }

}

void allTransactionsByDateDown(int userId, string selectedAccount)
{
    Console.WriteLine("Sve transakcije sortirane prema datumu silazno:");
    Console.WriteLine();
    var sortedFilteredTransactions = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount)
            .OrderByDescending(user => DateTime.ParseExact(user.Value.Item7, formatWithHoursAndMinutes, null))
            .ToDictionary(user => user.Key, user => user.Value);

    Console.WriteLine($"Tip - Iznos(eur) - Opis - Kategorija - Datum");
    Console.WriteLine();

    foreach (var transaction in sortedFilteredTransactions)
    {

        Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allTransactionsByDateDown(userId, selectedAccount);
            break;
    }

}

void allIncomeTransactions(int userId, string selectedAccount)
{
    Console.WriteLine("Sve transakcije koje imaju tip prihod:");
    Console.WriteLine();
    var sortedFilteredTransactions = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount && user.Value.Item3 == "prihod")
            .ToDictionary(user => user.Key, user => user.Value);

    Console.WriteLine($"Tip - Iznos(eur) - Opis - Kategorija - Datum");
    Console.WriteLine();

    foreach (var transaction in sortedFilteredTransactions)
    {

        Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allIncomeTransactions(userId, selectedAccount);
            break;
    }

}

void allExpenseTransactions(int userId, string selectedAccount)
{
    Console.WriteLine("Sve transakcije koje imaju tip rashod:");
    Console.WriteLine();
    var sortedFilteredTransactions = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount && user.Value.Item3 == "rashod")
            .ToDictionary(user => user.Key, user => user.Value);

    Console.WriteLine($"Tip - Iznos(eur) - Opis - Kategorija - Datum");
    Console.WriteLine();

    foreach (var transaction in sortedFilteredTransactions)
    {

        Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allExpenseTransactions(userId, selectedAccount);
            break;
    }

}

void allTransactionsByCategory(int userId, string selectedAccount)
{
    var decision = "";

    while (decision != "1" && decision != "2" && decision != "3" && decision != "4" && decision != "5" && decision != "6")
    {
        Console.WriteLine("1 - Placa");
        Console.WriteLine("2 - Honorar");
        Console.WriteLine("3 - Poklon");
        Console.WriteLine("4 - Hrana");
        Console.WriteLine("5 - Prijevoz");
        Console.WriteLine("6 - Sport");
        Console.WriteLine();
        Console.Write("Odaberite kategoriju za koju zelite vidjeti transakcije: ");
        decision = Console.ReadLine();

        if(decision != "1" && decision != "2" && decision != "3" && decision != "4" && decision != "5" && decision != "6")
        {
            wrongEntryy();
        }
    }

    switch(decision)
    {
        case "1":
            decision = "placa";
            break;
        case "2":
            decision = "honorar";
            break;
        case "3":
            decision = "poklon";
            break;
        case "4":
            decision = "hrana";
            break;
        case "5":
            decision = "prijevoz";
            break;
        case "6":
            decision = "sport";
            break;
    }
    
    Console.WriteLine();
    var sortedFilteredTransactions = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount && user.Value.Item6 == decision)
            .ToDictionary(user => user.Key, user => user.Value);

    Console.Clear();

    if(sortedFilteredTransactions.Count() < 1)
    {
        Console.WriteLine($"Nema transakcija za kategoriju {decision} na ovome racunu...");
    }
    else
    {
        Console.WriteLine($"Tip - Iznos(eur) - Opis - Kategorija - Datum");
        Console.WriteLine();
    }

    foreach (var transaction in sortedFilteredTransactions)
    {

        Console.WriteLine($"{transaction.Value.Item3} - {transaction.Value.Item4} - {transaction.Value.Item5} - {transaction.Value.Item6} - {transaction.Value.Item7}");
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision1 = Console.ReadLine();

    switch (decision1)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            allTransactionsByCategory(userId, selectedAccount);
            break;
    }

}

void accountBalance(int userId, string selectedAccount)
{
    decimal totalIncome = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount && user.Value.Item3 == "prihod")
            .Sum(user => user.Value.Item4);

    decimal totalExpense = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount && user.Value.Item3 == "rashod")
            .Sum(user => user.Value.Item4);

    decimal result = totalIncome - totalExpense;
    Console.WriteLine($"Trenutno stanje za {selectedAccount} racun: {result}eur");

    if(result < 0)
    {
        Console.WriteLine();
        Console.WriteLine("UPOZORENJE! Vas racun je u minusu.");
    }

    Console.WriteLine();    
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            accountBalance(userId, selectedAccount);
            return;
    }
}

void totalTransactions(int userId, string selectedAccount)
{
    int count = usersTransactions
            .Count(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount);

    Console.WriteLine($"Ukupan broj transakcija za {selectedAccount} racun: {count}");
    Console.WriteLine();
    Console.Write("Unesite 0 za povratak: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        default:
            wrongEntryy();
            totalTransactions(userId, selectedAccount);
            return;
    }
}

void incomeExpenseAmount(int userId, string selectedAccount)
{
    Console.Write("Unesite godinu za koju zelite vidjeti ukupan iznos prihoda i rashoda: ");
    var yearEntry = Console.ReadLine();
    Console.WriteLine();
    Console.Write("Unesite mjesec za koji zelite vidjeti ukupan iznos prihoda i rashoda: ");
    var monthEntry = Console.ReadLine();

    if(!int.TryParse(yearEntry, out _) || !int.TryParse(monthEntry, out _))
    {
        wrongEntryy();
        incomeExpenseAmount(userId, selectedAccount);
        return;
    }
    else if(int.Parse(monthEntry) > 12)
    {
        Console.Clear();
        Console.WriteLine("Pogresan unos, mjesec ne moze biti veci od 12. Pokusajte ponovno.");
        Console.WriteLine();
        incomeExpenseAmount(userId, selectedAccount);
        return;
    }
    else if(int.Parse(monthEntry) < 1 || int.Parse(yearEntry) < 1)
    {
        Console.Clear();
        Console.WriteLine("Pogresan unos, godina ili mjesec ne moze biti negativan broj. Pokusajte ponovno.");
        Console.WriteLine();
        incomeExpenseAmount(userId, selectedAccount);
        return;
    }

    decimal totalIncome = usersTransactions
            .Where(
                user => user.Value.Item1 == userId && 
                user.Value.Item2 == selectedAccount &&
                user.Value.Item3 == "prihod" &&              
                DateTime.TryParseExact(
                    user.Value.Item7, formatWithHoursAndMinutes, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) &&
                date.Month == int.Parse(monthEntry) &&             
                date.Year == int.Parse(yearEntry)                    
            )
            .Sum(user => user.Value.Item4);

    decimal totalExpense = usersTransactions
            .Where(
                user => user.Value.Item1 == userId &&
                user.Value.Item2 == selectedAccount &&
                user.Value.Item3 == "rashod" &&
                DateTime.TryParseExact(
                    user.Value.Item7, formatWithHoursAndMinutes, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) &&
                date.Month == int.Parse(monthEntry) &&
                date.Year == int.Parse(yearEntry)
            )
            .Sum(user => user.Value.Item4);

    Console.Clear();

    var decision = "";

    while(decision != "0")
    {
        Console.WriteLine($"Ukupan iznos prihoda za {monthEntry}. mjesec i {yearEntry}. godinu: {totalIncome}");
        Console.WriteLine($"Ukupan iznos rashoda za {monthEntry}. mjesec i {yearEntry}. godinu: {totalExpense}");
        Console.WriteLine();
        Console.Write("Unesite 0 za povratak: ");
        decision = Console.ReadLine();

        switch (decision)
        {
            case "0":
                Console.Clear();
                transactionsMenu(userId, selectedAccount);
                return;
            default:
                wrongEntryy();
                break;
        }
    }
    
}

void expensePercentage(int userId, string selectedAccount)
{
    var decision = "";

    while (decision != "1" && decision != "2" && decision != "3")
    {
        Console.WriteLine("1 - Hrana");
        Console.WriteLine("2 - Prijevoz");
        Console.WriteLine("3 - Sport");
        Console.WriteLine();
        Console.Write("Odaberite kategoriju za koju zelite vidjeti postotak udjela rashoda: ");
        decision = Console.ReadLine();

        if (decision != "1" && decision != "2" && decision != "3")
        {
            wrongEntryy();
        }
    }

    switch (decision)
    {
        case "1":
            decision = "hrana";
            break;
        case "2":
            decision = "prijevoz";
            break;
        case "3":
            decision = "sport";
            break;
    }

    decimal totalExpenses = usersTransactions
            .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount && user.Value.Item3 == "rashod")
            .Sum(user => user.Value.Item4);

    decimal categoryExpenses = usersTransactions
        .Where(user => user.Value.Item1 == userId && user.Value.Item2 == selectedAccount && user.Value.Item3 == "rashod" && user.Value.Item6 == decision)
        .Sum(user => user.Value.Item4);

    var decision1 = "";

    while (decision1 != "0")
    {
        if (totalExpenses > 0)
        {
            decimal percentage = (categoryExpenses / totalExpenses) * 100;
            Console.Clear();
            Console.WriteLine($"Postotak udjela rashoda za kategoriju {decision}: {percentage:F2}%");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Nema rashoda za izračun.");
        }

        Console.WriteLine();
        Console.Write("Unesite 0 za povratak: ");
        decision1 = Console.ReadLine();

        switch (decision1)
        {
            case "0":
                Console.Clear();
                transactionsMenu(userId, selectedAccount);
                return;
            default:
                wrongEntryy();
                break;
        }
    }
}

void transactionAveragePerDate(int userId, string selectedAccount)
{
    Console.Write("Unesite godinu za koju zelite vidjeti prosjecan iznos transakcije: ");
    var yearEntry = Console.ReadLine();
    Console.WriteLine();
    Console.Write("Unesite mjesec za koji zelite vidjeti prosjecan iznos transakcije: ");
    var monthEntry = Console.ReadLine();

    if (!int.TryParse(yearEntry, out _) || !int.TryParse(monthEntry, out _))
    {
        wrongEntryy();
        transactionAveragePerDate(userId, selectedAccount);
        return;
    }
    else if (int.Parse(monthEntry) > 12)
    {
        Console.Clear();
        Console.WriteLine("Pogresan unos, mjesec ne moze biti veci od 12. Pokusajte ponovno.");
        Console.WriteLine();
        transactionAveragePerDate(userId, selectedAccount);
        return;
    }
    else if (int.Parse(monthEntry) < 1 || int.Parse(yearEntry) < 1)
    {
        Console.Clear();
        Console.WriteLine("Pogresan unos, godina ili mjesec ne moze biti negativan broj. Pokusajte ponovno.");
        Console.WriteLine();
        transactionAveragePerDate(userId, selectedAccount);
        return;
    }

    decimal averageAmount = usersTransactions
            .Where(
                user => user.Value.Item1 == userId &&
                user.Value.Item2 == selectedAccount &&
                DateTime.TryParseExact(
                    user.Value.Item7, formatWithHoursAndMinutes, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) &&
                date.Month == int.Parse(monthEntry) &&
                date.Year == int.Parse(yearEntry)
            )
            .Average(user => user.Value.Item4);

    Console.Clear();

    var decision = "";

    while (decision != "0")
    {
        Console.WriteLine($"Prosjecan iznos transakcije za {monthEntry}. mjesec i {yearEntry}. godinu: {averageAmount}");
        Console.WriteLine();
        Console.Write("Unesite 0 za povratak: ");
        decision = Console.ReadLine();

        switch (decision)
        {
            case "0":
                Console.Clear();
                transactionsMenu(userId, selectedAccount);
                return;
            default:
                wrongEntryy();
                break;
        }
    }

}

void transactionAveragePerCategory(int userId, string selectedAccount)
{
    var decision = "";

    while (decision != "1" && decision != "2" && decision != "3" && decision != "4" && decision != "5" && decision != "6")
    {
        Console.WriteLine("1 - Placa");
        Console.WriteLine("2 - Honorar");
        Console.WriteLine("3 - Poklon");
        Console.WriteLine("4 - Hrana");
        Console.WriteLine("5 - Prijevoz");
        Console.WriteLine("6 - Sport");
        Console.WriteLine();
        Console.Write("Odaberite kategoriju za koju zelite vidjeti transakcije: ");
        decision = Console.ReadLine();

        if (decision != "1" && decision != "2" && decision != "3" && decision != "4" && decision != "5" && decision != "6")
        {
            wrongEntryy();
        }
    }

    switch (decision)
    {
        case "1":
            decision = "placa";
            break;
        case "2":
            decision = "honorar";
            break;
        case "3":
            decision = "poklon";
            break;
        case "4":
            decision = "hrana";
            break;
        case "5":
            decision = "prijevoz";
            break;
        case "6":
            decision = "sport";
            break;
    }

    decimal averageAmount = usersTransactions
            .Where(
                user => user.Value.Item1 == userId &&
                user.Value.Item2 == selectedAccount &&
                user.Value.Item6 == decision
            )
            .Average(user => user.Value.Item4);

    Console.Clear();

    var decision1 = "";

    while (decision1 != "0")
    {
        Console.WriteLine($"Prosjecan iznos transakcije za kategoriju {decision}: {averageAmount}");
        Console.WriteLine();
        Console.Write("Unesite 0 za povratak: ");
        decision = Console.ReadLine();

        switch (decision1)
        {
            case "0":
                Console.Clear();
                transactionsMenu(userId, selectedAccount);
                return;
            default:
                wrongEntryy();
                break;
        }
    }

}

void financialReport(int userId, string selectedAccount)
{
    Console.WriteLine($"{users[userId].Item1} {users[userId].Item2} => {selectedAccount} => financijsko izvjesce");
    Console.WriteLine();
    Console.WriteLine("1 - Stanje racuna");
    Console.WriteLine("2 - Ukupni broj transakcija");
    Console.WriteLine("3 - Iznos prihoda i rashoda za odredeni mjesec i godinu");
    Console.WriteLine("4 - Postotak udjela rashoda za odredenu kategoriju");
    Console.WriteLine("5 - Prosjecni iznos transakcije za odredeni mjesec i godinu");
    Console.WriteLine("6 - Prosjecni iznos transakcije za odabranu kategoriju");
    Console.WriteLine("7 - Natrag");
    Console.WriteLine("0 - Pocetni meni");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            accountBalance(userId, selectedAccount);
            return;
        case "2":
            Console.Clear();
            totalTransactions(userId, selectedAccount);
            return;
        case "3":
            Console.Clear();
            incomeExpenseAmount(userId, selectedAccount);
            return;
        case "4":
            Console.Clear();
            expensePercentage(userId, selectedAccount);
            return;
        case "5":
            Console.Clear();
            transactionAveragePerDate(userId, selectedAccount);
            return;
        case "6":
            Console.Clear();
            transactionAveragePerCategory(userId, selectedAccount);
            return;
        case "7":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            financialReport(userId, selectedAccount);
            return;
    }
}

void reviewTransactions(int userId, string selectedAccount) 
{
    Console.WriteLine($"{users[userId].Item1} {users[userId].Item2} => {selectedAccount} => pregled transakcija");
    Console.WriteLine();
    Console.WriteLine("1 - Sve transakcije");
    Console.WriteLine("2 - Sve transakcije sortirane po iznosu uzlazno");
    Console.WriteLine("3 - Sve transakcije sortirane po iznosu silazno");
    Console.WriteLine("4 - Sve transakcije sortirane po opisu abecedno");
    Console.WriteLine("5 - Sve transakcije sortirane po datumu uzlazno");
    Console.WriteLine("6 - Sve transakcije sortirane po datumu silazno");
    Console.WriteLine("7 - Svi prihodi");
    Console.WriteLine("8 - Svi rashodi");
    Console.WriteLine("9 - Sve transakcije za odabranu kategoriju");
    Console.WriteLine("10 - Natrag");
    Console.WriteLine("0 - Pocetni meni");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision) 
    {
        case "1":
            Console.Clear();
            allTransactions(userId, selectedAccount);
            return;
        case "2":
            Console.Clear();
            allTransactionsByAmountUp(userId, selectedAccount);
            return;
        case "3":
            Console.Clear();
            allTransactionsByAmountDown(userId,selectedAccount);
            return;
        case "4":
            Console.Clear();
            allTransactionsByDescription(userId,selectedAccount);
            return;
        case "5":
            Console.Clear();
            allTransactionsByDateUp(userId, selectedAccount);
            return;
        case "6":
            Console.Clear();
            allTransactionsByDateDown(userId, selectedAccount);
            return;
        case "7":
            Console.Clear();
            allIncomeTransactions(userId, selectedAccount);
            return;
        case "8":
            Console.Clear();
            allExpenseTransactions(userId, selectedAccount);
            return;
        case "9":
            Console.Clear();
            allTransactionsByCategory(userId, selectedAccount);
            return;
        case "10":
            Console.Clear();
            transactionsMenu(userId, selectedAccount);
            return;            
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            reviewTransactions(userId, selectedAccount);
            return;
    }
}

void transactionsMenu(int userId, string selectedAccount) 
{
    Console.WriteLine($"{users[userId].Item1} {users[userId].Item2} => {selectedAccount}");
    Console.WriteLine();
    Console.WriteLine("1 - Unos nove transakcije");
    Console.WriteLine("2 - Brisanje transakcije");
    Console.WriteLine("3 - Uredivanje transakcije");
    Console.WriteLine("4 - Pregled transakcija");
    Console.WriteLine("5 - Financijsko izvjesce");
    Console.WriteLine("6 - Natrag");
    Console.WriteLine("0 - Pocetni meni");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            createTransaction(userId, selectedAccount);
            return;
        case "2":
            Console.Clear();
            deleteTransactions(userId, selectedAccount);
            return;
        case "3":
            Console.Clear();
            updateTransaction(userId, selectedAccount);
            return;
        case "4":
            Console.Clear();
            reviewTransactions(userId, selectedAccount);
            return;
        case "5":
            Console.Clear();
            financialReport(userId, selectedAccount);
            return;
        case "6":
            Console.Clear();
            account(userId);
            return;
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default: 
            wrongEntryy();
            transactionsMenu(userId, selectedAccount);
            return;
    }
}

void chooseAccount(int userId) 
{
    Console.WriteLine($"{users[userId].Item1} {users[userId].Item2}");
    Console.WriteLine();
    Console.WriteLine("1 - Tekuci racun");
    Console.WriteLine("2 - Ziro racun");
    Console.WriteLine("3 - Prepaid racun");
    Console.WriteLine("0 - Natrag");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

   

    switch (decision)
    {
        case "1":
            Console.Clear();
            transactionsMenu(userId, "tekuci");
            return;
        case "2":
            Console.Clear();
            transactionsMenu(userId, "ziro");
            return;
        case "3":
            Console.Clear();
            transactionsMenu(userId, "prepaid");
            return;
        case "0":
            Console.Clear();
            account(userId);
            return;
        default:
            wrongEntryy();
            chooseAccount(userId);
            return;
    }

}

// Korisnicki racun meni
void account(int userId) 
{
    Console.WriteLine($"{users[userId].Item1} {users[userId].Item2}");
    Console.WriteLine();
    Console.WriteLine("1 - Pregled racuna");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision) 
    {
        case "1": 
            Console.Clear();
            chooseAccount(userId);
            return;
        case "0": 
            Console.Clear();
            startingMenu();
            return;
        default: 
            wrongEntryy();
            account(userId);
            return;
    }
}

// Logiranje u racun korisnika
void accountLogin() 
{
    Console.WriteLine("Unesite ime korisnika ili unesite 0 za povratak na pocetni meni: ");
    var name = Console.ReadLine();

    if (name == "0")
    {
        Console.Clear();
        startingMenu();
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Unesite prezime korisnika: ");
    var lastname = Console.ReadLine();
    int userId;

    

    foreach (var user in users)
    {
        if (user.Value.Item1.ToLower() == name.ToLower() && user.Value.Item2.ToLower() == lastname.ToLower()) 
        {
            Console.Clear();
            Console.WriteLine($"Dobrodosli {user.Value.Item1} {user.Value.Item2}!");
            Console.WriteLine();
            userId = user.Key;
            account(userId);
            return;
        }
    }

    wrongEntryy();
    accountLogin();
    
}

// Pocetni meni
void startingMenu()
{
    Console.WriteLine("1 - Korisnici");
    Console.WriteLine("2 - Racuni");
    Console.WriteLine("3 - Izlaz iz aplikacije");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            usersMenu();
            return;
        case "2":
            Console.Clear();
            accountLogin();
            return;
        case "3":
            return;
        default:
            wrongEntryy();
            startingMenu();
            return;
    }
}

startingMenu();


