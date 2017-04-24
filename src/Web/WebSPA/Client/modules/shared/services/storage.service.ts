import { Injectable } from '@angular/core';

@Injectable()
export class StorageService {
    private storage: any;

    constructor() {
        this.storage = sessionStorage; // localStorage;
    }

    public retrieve(key: string): any {
        let item = this.storage.getItem(key);

        if (item && item !== 'undefined') {
            return JSON.parse(this.storage.getItem(key));
        }

        return;
    }

    public store(key: string, value: any) {
        this.storage.setItem(key, JSON.stringify(value));
    }
}
