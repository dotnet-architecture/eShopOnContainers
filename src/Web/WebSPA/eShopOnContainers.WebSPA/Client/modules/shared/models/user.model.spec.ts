import { User } from './user.model';
// todo: I dont think user follows angular style guides

describe('User Model', () => {
  it('has displayName', () => {
    let userModel: User = {displayName: 'test', roles: ['1']};
    expect(userModel.displayName).toEqual('test');
  });
  it('has displayName', () => {
    let userModel: User = {displayName: 'test', roles: ['admin']};
    expect(userModel.roles[0]).toEqual('admin');
  });
});
