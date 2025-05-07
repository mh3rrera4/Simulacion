class GeneradorCongruenciaMixto{
	private int a = 1664525;
	private int c = 1013904223;
	private int m = (int)Math.Pow(2,32);
	private int semilla;
	
	public GeneradorCongruenciaMixto(int semillaTotal){
        semilla = semillaTotal;
    }

	public int GenerarNumero(){
	    semilla = (a * semilla + c) % m;
	    return semilla;
	}
}