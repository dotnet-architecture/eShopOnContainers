import { ControlBase } from './control-base';

export class ControlDropdown extends ControlBase<string> {
    options: { key: string, value: string }[] = [];

    constructor(options: any = {}) {
        super(options);
        this.type = 'dropdown';
        this.options = options.options || [];
    }
}
