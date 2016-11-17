import { Injectable } from '@angular/core';

import { ApiGatewayService } from './api-gateway.service';

@Injectable()
export class DataService {

    constructor(public http: ApiGatewayService) { }

    get(url: string, params?: any) {
        return this.http.get(url, undefined);
    }

    post(url: string, data: any, params?: any) {
        return this.http.post(url, data, params);
    }
}
