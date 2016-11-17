import { Injectable } from '@angular/core';

import { DataService } from './data.service';

@Injectable()
export class ContentService {

    constructor(public dataService: DataService) { }

    get(lang?: string): any {
        return this.dataService.get('api/content?lang=' + (lang ? lang : 'en'));
    }
}
