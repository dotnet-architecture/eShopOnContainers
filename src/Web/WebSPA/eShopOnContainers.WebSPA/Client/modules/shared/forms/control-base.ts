export class ControlBase<T>{
    value: T;
    key: string;
    label: string;
    placeholder: string;
    required: boolean;
    minlength: number;
    maxlength: number;
    order: number;
    type: string;
    class: string;

    constructor(options: {
        value?: T,
        key?: string,
        label?: string,
        placeholder?: string,
        required?: boolean,
        minlength?: number,
        maxlength?: number,
        order?: number,
        type?: string,
        class?: string;
    } = {}) {
        this.value = options.value;
        this.key = options.key || '';
        this.label = options.label || '';
        this.placeholder = options.placeholder || '';
        this.required = !!options.required;
        this.minlength = options.minlength;
        this.maxlength = options.maxlength;
        this.order = options.order === undefined ? 1 : options.order;
        this.type = options.type || '';
        this.class = options.class || '';
    }
}
