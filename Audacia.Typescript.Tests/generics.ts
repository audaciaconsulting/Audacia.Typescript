class Wrapper<T extends { length:string, name:string }, T2> {
    value: T;
    otherValue: T2;

    public longest<TOther extends Wrapper<T, T2>>(input: TOther): Wrapper<T, T2> {
        if (this.value.length > input.value.length) return this;
        else return input;
    }
}