using System;

namespace BankApp {

    class AcntHolder {
        private string name;
        private string birthday;
        private string ssn;

        public AcntHolder(string name, string birthday, string ssn){
            this.name = name;
            this.birthday = birthday;
            this.ssn = ssn;
        }

        public string getName(){
            return this.name;
        }
        public string getBirthday(){
            return this.birthday;
        }

        //This function is not a simple get function because this is sensitive personal data
        public bool validateSSN(string ssn){
            return this.ssn == ssn;
        }
    }

    class Account {
        //if type is Individual or Corporate, it is also an Investment Account
        public enum type {
            Checking,
            Individual,
            Corporate
        }
        public type acntType;

        private AcntHolder owner;
        private float amountInAccount;
        private bool isLocked;

        //private Account() { }

        public Account(type t, AcntHolder owner, float initialDeposit){
            this.acntType = t;
            this.owner = owner;
            this.amountInAccount = initialDeposit;
            this.isLocked = false;
        }

        internal void addFunds(float amount){
            this.amountInAccount += amount;
        }

        internal void subtractFunds(float amount){
            this.amountInAccount -= amount;
        }

        internal string getOwnerName(){
            return this.owner.getName();
        }

        internal float getCurrentAmount(){
            return this.amountInAccount;
        }

        /*returns 1  if successful, 
          returns 0  if account has insufficient funds, (if applicable)
          returns -1 if amount is not positive,
          returns -2 if account is locked*/
        private int validateTransfer(string transactionType, float amount){
            transactionType = char.ToUpper(transactionType[0]) + transactionType.Substring(1);

            if(this.isLocked){
                Console.WriteLine("Account is locked. " + transactionType + " is not possible.\nTransaction has been canceled.");
                return -2;
            }
            if(amount <= 0){
                Console.WriteLine(transactionType + " Error: " + transactionType + " amount should be greater than 0.\nTransaction has been canceled.");
                return -1;
            }
            if(transactionType != "Deposit" && this.amountInAccount < amount){
                Console.WriteLine(transactionType + " Error: Insufficient Funds in account.\nTotal funds: $" + amountInAccount + "\n" + transactionType + " amount: $" + amount + "\nTransaction has been canceled.");
                return 0;
            }

            return 1;
        }

        public int transferTo(Account receiver, float amount){
            int check = this.validateTransfer("Transfer", amount);

            if(check < 1)
                return check;
            
            this.subtractFunds(amount);
            receiver.addFunds(amount);

            Console.WriteLine("Notification: Transfer of $" + amount + " to " + receiver.getOwnerName() + " successful.");
            return check;
        }

        
        public int withdraw(float amount){
            int check = this.validateTransfer("Withdrawal", amount);
            if(check < 1)
                return check;

            //displays a warning, but transaction will go through with the limited amount
            if(this.acntType == Account.type.Individual && amount > 500){
                Console.WriteLine("Notification: Individual Investment accounts have a withdrawal limit of $500.00.\nThis transaction has been set to this limit.");
                amount = 500f;
            }

            this.subtractFunds(amount);
            Console.WriteLine("Notification: Withdrawal of $" + amount + " successful.");
            return 1;
        }

        public int deposit(float amount){
            int check = this.validateTransfer("Deposit", amount);
            if(check < 1)
                return check;

            this.addFunds(amount);
            Console.WriteLine("Notification: Deposit of $" + amount + " successful.");
            return 1;
        }
    }

    class BankDemo{

        public static void printStatus(Account a, Account s, Account c){
            Console.WriteLine("Alex has $" + a.getCurrentAmount() + " in their account.");
            Console.WriteLine("Steve has $" + s.getCurrentAmount() + " in their account.");
            Console.WriteLine("Freddy Corp has $" + c.getCurrentAmount() + " in its account.\n");
        }
        
        static void Main(string[] args){
            //set up accounts
            AcntHolder alex = new AcntHolder("Alex Smith", "9/9/95", "999-99-9999");
            Account alexAcnt = new Account(Account.type.Individual, alex, 853.00f);

            AcntHolder steve = new AcntHolder("Steve Barnes", "11/11/94", "XXX-XX-XXXX");
            Account steveAcnt = new Account(Account.type.Checking, steve, 1257.00f);

            AcntHolder corp = new AcntHolder("Freddy Corp", "8/7/65", "123-45-6789");
            Account corpAcnt = new Account(Account.type.Corporate, corp, 1692037.00f);

            printStatus(alexAcnt, steveAcnt, corpAcnt);

            Console.WriteLine("Alex tries to make a withdrawal of $600.\nSteve makes a deposit of $37.49.");
            Console.WriteLine("Alex will have $353 left in their account, and Steve will have $1,294.49.\n");

            alexAcnt.withdraw(600f);
            steveAcnt.deposit(37.49f);

            printStatus(alexAcnt, steveAcnt, corpAcnt);

            Console.WriteLine("Alex tries to make a withdrawal of $1000.\nSteve tries to make a deposit of -$10.");
            Console.WriteLine("Neither account will see any change.\n");

            alexAcnt.withdraw(1000f);
            steveAcnt.deposit(-10f);

            printStatus(alexAcnt, steveAcnt, corpAcnt);

            Console.WriteLine("Freddy Corp pays both of its employees their weekly salaries of $1,100.");
            Console.WriteLine("Alex will have $1,453, and Steve will have $2,394.49. The corporate account will have $1,689,837.\n");

            corpAcnt.transferTo(alexAcnt, 1100f);
            corpAcnt.transferTo(steveAcnt, 1100f);

            printStatus(alexAcnt, steveAcnt, corpAcnt);
            
        }
    }
}