package org.herac.tuxguitar.gui.tools.collab;

import org.eclipse.swt.SWT;
import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.List;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.Text;
import org.eclipse.swt.widgets.List;
import org.herac.tuxguitar.gui.TuxGuitar;
import org.herac.tuxguitar.gui.util.DialogUtils;
import org.herac.tuxguitar.gui.util.MessageDialog;
import java.util.ArrayList;

public class SessionDialog {
	
	public void show() {
		
		final Shell dialog = DialogUtils.newDialog(TuxGuitar.instance().getShell(), SWT.DIALOG_TRIM | SWT.APPLICATION_MODAL);
		dialog.setLayout(new GridLayout());
		dialog.setText("Choose or create a new session");
		dialog.setLayoutData(new GridData(SWT.FILL,SWT.FILL,true,true));
		
		// ----------------------------------------------------------------------
		
		Composite composite = new Composite(dialog, SWT.NONE);
		composite.setLayout(new GridLayout(2,false));
		
		//-------USER------------------------------------
		Label labelUser = new Label(composite,SWT.LEFT);
		labelUser.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		labelUser.setText("User:");
		
		Label labelName = new Label(composite,SWT.LEFT| SWT.WRAP);
		labelName.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		labelName.setText(TuxGuitar.instance().clienteEnvia.getLogin());

		
		//-------SESSION ------------------------------------
		
		Label labelSession = new Label(composite,SWT.LEFT);
		labelSession.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		labelSession.setText("Session name:");

		final Text TxtSession = new Text(composite,SWT.BORDER | SWT.LEFT | SWT.WRAP);
		TxtSession.setLayoutData(new GridData(300,SWT.DEFAULT));
		TxtSession.setText("");
		
		//-------CURRENT SESSIONS------------------------------------
		
		Label labelS = new Label(composite,SWT.LEFT);
		labelS.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		labelS.setText("Current Sessions:");
		
		final List sessions = new List(composite,SWT.LEFT | SWT.BORDER | SWT.V_SCROLL);
		sessions.setLayoutData(new GridData(250,200));
		//sessions.setLayoutData(new GridData(SWT.LEFT,SWT.TOP,false,true));
		
		for(int i =0 ;i< TuxGuitar.instance().clienteEnvia.listaSessoes.size();i++)
		{
			sessions.add((String) TuxGuitar.instance().clienteEnvia.listaSessoes.get(i) );
		}

		// sessions.select(TuxGuitar.instance().getScaleManager().getSelectionIndex() + 1);
		// Aqui é programado a seleção do elemento em uma lista
		sessions.addSelectionListener(new SelectionAdapter() {
			public void widgetSelected(SelectionEvent arg0) 
			{
				
				String selected[] = sessions.getSelection();
		        for (int i = 0; i < selected.length; i++) 
		        {
		        	TxtSession.setText(selected[i]);
		        }
				
			}
		});
		

		
		//------------------BUTTONS--------------------------
		Composite buttons = new Composite(dialog, SWT.NONE);
		buttons.setLayout(new GridLayout(2,false));
		buttons.setLayoutData(new GridData(SWT.END,SWT.FILL,true,true));
		
		final Button buttonOK = new Button(buttons, SWT.PUSH);
		buttonOK.setText("Start");
		buttonOK.setLayoutData(getButtonData());
		buttonOK.addSelectionListener(new SelectionAdapter() {
			public void widgetSelected(SelectionEvent arg0) 
			{
			
				try 
				{
					if( TxtSession.getText().equals("") )
					{
						// Mandar mensagem de erro
						MessageDialog.infoMessage("Session Dialog","Choose or create a new session");
						return;
						
					}
					else
					{
						//TODO: Validar o nome da sessão
						//TODO: Validar o nome do usuário

						// Entra em uma sessão existente
		            	ArrayList l = new ArrayList();
			        	l.add(TxtSession.getText()); 

						// Se não selecionaou nada é por que é uma nova sessão!
						if(sessions.getSelectionIndex() == -1)
						{
				    		//  Enviando para o argo uma mensagem de 'protocolo'
				        	TuxGuitar.instance().clienteEnvia.EnviaEvento(l,"PROT_nova_sessao"); 
						}
						else
						{
			    			//	Enviando para o argo uma mensagem de 'protocolo'
							TuxGuitar.instance().clienteEnvia.EnviaEvento(l,"PROT_sessao_existente");
						}
						
					}

					dialog.dispose();

				}
                catch (Exception erro) 
                {
                	erro.printStackTrace();
                }
                
			}
		});
		
		Button buttonCancel = new Button(buttons, SWT.PUSH);
		buttonCancel.setText("Close");
		buttonCancel.setLayoutData(getButtonData());
		buttonCancel.addSelectionListener(new SelectionAdapter() {
			public void widgetSelected(SelectionEvent arg0) {
				dialog.dispose();
			}
		});
		
		dialog.setDefaultButton( buttonOK );
		
		DialogUtils.openDialog(dialog,DialogUtils.OPEN_STYLE_CENTER | DialogUtils.OPEN_STYLE_PACK | DialogUtils.OPEN_STYLE_WAIT);
	}
	
	private GridData getButtonData(){
		GridData data = new GridData(SWT.FILL, SWT.FILL, true, true);
		data.minimumWidth = 80;
		data.minimumHeight = 25;
		return data;
	}
}
