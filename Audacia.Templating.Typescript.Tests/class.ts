class College {
    city: string;
    name: string;

    constructor(name: string, city: string) {
        this.city = city;
        this.name = name;
    }

    public address(streetName: string) {
        return ('College Name:' + this.name + ' City: ' + this.city + ' Street Name: ' + streetName);
    }
}
