abstract class University {
    city: string;
    name: string;

    constructor(name: string, city: string) {
        this.city = city;
        this.name = name;
    }

    abstract thing(): string;
    public abstract address(streetName: string);
    age(): number {
        return 12;
    }
}