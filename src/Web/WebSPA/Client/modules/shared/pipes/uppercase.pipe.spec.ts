import { UppercasePipe } from './uppercase.pipe';

describe('Pipe appfUppercase', () => {
  let pipe: UppercasePipe;

  beforeEach(() => {
    pipe = new UppercasePipe();
  });

  it('transforms "abc" to "ABC"', () => {
    expect(pipe.transform('abc')).toEqual('ABC');
  });

  it('transforms "abc def" to "ABC DEF"', () => {
    expect(pipe.transform('abc def')).toEqual('ABC DEF');
  });

  it('leaves "ABC DEF" unchanged', () => {
    expect(pipe.transform('ABC DEF')).toEqual('ABC DEF');
  });
});
