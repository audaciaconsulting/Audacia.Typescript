class College {
    city: string;
    name: string = "Nigel";

    constructor(city: string) {
        this.city = city;
    }

    public address(streetName: string) {
        return ('College Name:' + this.name + ' City: ' + this.city + ' Street Name: ' + streetName);
    }
}