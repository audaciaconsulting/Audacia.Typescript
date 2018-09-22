// Interface
interface IStudent {
    yearOfBirth: number;
    age: number;
    printDetails(): void;
}

// Base class
class School {
    name: string;
    city: string;

    constructor(name: string, city: string) {

    }

    public address(streetName: string) {
        return ('College Name:' + this.name + ' City: ' + this.city + ' Street Name: ' + streetName);
    }
}

// Child Class implements IStudent and inherits from College
class Student extends School implements IStudent, IArguments {
    length: number;
    callee: Function;
    firstName: string;
    lastName: string;
    yearOfBirth: number;

    // Constructor
    constructor(firstName: string, lastName: string, name: string, city: string, yearOfBirth: number) {
        super(name, city);
        this.firstName = firstName;
        this.lastName = lastName;
        this.yearOfBirth = yearOfBirth;
    }

    collegeDetails() {
        var y = super.address('Maple Street');
        alert(y);
    }

    printDetails(): void {
        alert(this.firstName + ' ' + this.lastName + ' College is: ' + this.name);
    }

    get age(): number {
        return 2014 - this.yearOfBirth;
    }
    set age(value: number) {
        // Do a thing
        var self = this;
        function nested() {
            interface IAlsoNested extends IStudent, IArguments {
                faveColor: string;
            }

            return self.firstName;
        }

        nested();
    }
}