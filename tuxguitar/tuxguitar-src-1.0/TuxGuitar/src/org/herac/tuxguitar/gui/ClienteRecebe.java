// $Id: Wizard.java,v 1.7 2004/03/06 21:16:21 mvw Exp $
// Copyright (c) 1996-99 The Regents of the University of California. All
// Rights Reserved. Permission to use, copy, modify, and distribute this
// software and its documentation without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph appear in all copies.  This software program and
// documentation are copyrighted by The Regents of the University of
// California. The software program and documentation are supplied "AS
// IS", without any accompanying services from The Regents. The Regents
// does not warrant that the operation of the program will be
// uninterrupted or error-free. The end-user understands that the program
// was developed for research purposes and is advised not to rely
// exclusively on the program for any reason.  IN NO EVENT SHALL THE
// UNIVERSITY OF CALIFORNIA BE LIABLE TO ANY PARTY FOR DIRECT, INDIRECT,
// SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, INCLUDING LOST PROFITS,
// ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS DOCUMENTATION, EVEN IF
// THE UNIVERSITY OF CALIFORNIA HAS BEEN ADVISED OF THE POSSIBILITY OF
// SUCH DAMAGE. THE UNIVERSITY OF CALIFORNIA SPECIFICALLY DISCLAIMS ANY
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE
// PROVIDED HEREUNDER IS ON AN "AS IS" BASIS, AND THE UNIVERSITY OF
// CALIFORNIA HAS NO OBLIGATIONS TO PROVIDE MAINTENANCE, SUPPORT,
// UPDATES, ENHANCEMENTS, OR MODIFICATIONS. 


// File: Wizard.java
// Classes: Wizard
// Original Author: jrobbins@ics.uci.edu
// $Id: Wizard.java,v 1.7 2004/03/06 21:16:21 mvw Exp $

package org.herac.tuxguitar.gui;

import org.eclipse.swt.SWT;
import org.eclipse.swt.graphics.Color;
import org.eclipse.swt.events.MouseEvent;
import org.eclipse.swt.events.MouseListener;
import org.eclipse.swt.events.PaintEvent;
import org.eclipse.swt.events.PaintListener;
import org.eclipse.swt.events.SelectionAdapter;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.graphics.GC;
import org.eclipse.swt.graphics.Image;
import org.eclipse.swt.graphics.Point;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Composite;
import org.eclipse.swt.widgets.Label;
import org.herac.tuxguitar.gui.TuxGuitar;
import org.herac.tuxguitar.gui.actions.ActionLock;
import org.herac.tuxguitar.gui.actions.caret.GoLeftAction;
import org.herac.tuxguitar.gui.actions.caret.GoRightAction;
import org.herac.tuxguitar.gui.actions.duration.DecrementDurationAction;
import org.herac.tuxguitar.gui.actions.duration.IncrementDurationAction;
import org.herac.tuxguitar.gui.actions.tools.ScaleAction;
import org.herac.tuxguitar.gui.editors.TGPainter;
import org.herac.tuxguitar.gui.editors.tab.Caret;
import org.herac.tuxguitar.gui.editors.tab.TGNoteImpl;
import org.herac.tuxguitar.gui.undo.undoables.measure.UndoableMeasureGeneric;
import org.herac.tuxguitar.gui.undo.undoables.measure.UndoableAddMeasure;
import org.herac.tuxguitar.song.managers.TGSongManager;
import org.herac.tuxguitar.song.models.TGBeat;
import org.herac.tuxguitar.song.models.TGDuration;
import org.herac.tuxguitar.song.models.TGNote;
import org.herac.tuxguitar.song.models.TGString;
import org.herac.tuxguitar.song.models.TGTrack;


import javax.swing.JOptionPane;

import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.StringBufferInputStream;
import java.net.URL;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Vector;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;
import java.awt.event.ActionEvent;
import java.awt.*;
import java.awt.event.KeyEvent;



import javax.swing.event.EventListenerList;
import javax.xml.parsers.ParserConfigurationException;
import javax.swing.Action;
import javax.swing.Icon;

import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

//Estes imports são úteis para a colaboração!
import java.net.*;
import java.io.*;
import java.util.*;
import java.awt.*;
import java.util.List;
import java.util.Collection;


public class ClienteRecebe extends Thread 
{
	
	private Socket socket;
	private DataInputStream is;
	// private Editor e;
	
	public ClienteRecebe(Socket s,DataInputStream i) {
		this.socket = s;
		this.is = i;
	}
	
    public void run ()  {
        
    	try {

            boolean clientTalking = true;
            
            //a loop that reads from and writes to the socket
            while (clientTalking) {
            	
            	byte[] clientObject = new byte[100];
            	//get what client wants to say...
                
            	this.is.read(clientObject);
            	
            	String msg = new String(clientObject);
            	
            	// System.out.println("Mensagem Completa:" + msg);
            	
            	String [] dados = msg.split(";");

            	String nomeEvento = dados[0];
            	
            	
            	
            	// Aqui vou tocar a nota e avançar o cursor
               	if (nomeEvento.startsWith("PlayNote"))
            	{
               		String user = (String) dados[3];
               		
               		TuxGuitar.instance().clienteEnvia.setCorAtual( TuxGuitar.instance().clienteEnvia.retornaCor(user));
               		
                	this.TocaNota(dados[1],user);
                   		
            	}
                
               	if (nomeEvento.startsWith("PROT_atualiza_modelo_cliente_inicial"))
            	{
            	}
               	
               	
               	// Recebeu uma mensagem de chat!
               	if (nomeEvento.startsWith("PROT_chat_msg"))
            	{
               		/*
               		ProjectBrowser.getInstance().getStatusBar().showStatusBlink("Nova mensagem de chat!",(Color) list.get(2));
               		
               		TabCht t = (TabCht)	ProjectBrowser.getInstance().getNamedTab("Chat");
        			t.setTexto((String) o, (Color) list.get(2));
        			*/

            	}

//              Recebeu a notificação que algum cliente entrou na da sessão!
               	if (nomeEvento.startsWith("PROT_inicio_sessao"))
            	{
               		/*
               		ProjectBrowser.getInstance().getStatusBar().showStatusBlink("Usuário " + ((String) o) + " entrou na sessão.");
               		*/
            	}

               	
//              Recebeu a notificação que algum cliente saiu da sessão!
               	if (nomeEvento.startsWith("PROT_fim_sessao"))
            	{
               		/*
               		ProjectBrowser.getInstance().getStatusBar().showStatusBlink("Usuário " + ((String) o) + " saiu da sessão.");
               		*/
            	}
               	

                // Esta ação foi removida para a experiência
               	/* if (nomeEvento.equals("ActionDeleteFromDiagram-actionPerformed"))
                	ActionDeleteFromDiagram.SINGLETON.actionPerformedImpl((ActionEvent) o); */
                
                if (nomeEvento.startsWith("PROT_remove_elemento"))
                {
                	/* 
                	ActionRemoveFromModel.SINGLETON.actionPerformedImpl((ArrayList) o);
                	*/
                }
                
               	if (nomeEvento.startsWith("SEL_"))
            	{
               		/*
               		nomeEvento = nomeEvento.substring(4,nomeEvento.length());
               		
               		SelecionaTool(nomeEvento,o,false);
               		*/
                    
            	}
               	                
                if (clientObject == null) 
                    clientTalking = false;
            }
           	
            } catch (Exception e) {
            	// Retirado para evitar mostrar o erro da desconexão!
            	e.printStackTrace();
            }    	
    }
    
    
    private void TocaNota(String nota, String user)
    {
    	
   		// System.out.println("Mensagem:" + msg);
    	
    	TuxGuitar.instance().lock();
    	ActionLock.lock();
    	
    	if(!this.AddNote(nota,user))
    	{
    		System.out.print("Nota Erro");
    		return;
    	}
    	
    	this.afterAction();
    	ActionLock.unlock();
    	TuxGuitar.instance().unlock();
    	
     	MoveDireita();
    	// TuxGuitar.instance().getAction(GoRightAction.NAME).process(null);
    	// MoveDireita();
    	
		// Este código está em afterActions() de Piano.java
    }
    
    protected boolean AddNote(String nota,String user) 
	{
    	
    	int value = getValue(nota);
    	TGBeat beat = TuxGuitar.instance().getEditorCache().getEditBeat();
    	
		Caret caret = TuxGuitar.instance().getTablatureEditor().getTablature().getCaret();
		
		// Verifico se preciso mudar a track (pista) ou não
		this.MudaTrack(user,caret);
		
		List strings = caret.getTrack().getStrings();
		for(int i = 0;i < strings.size();i ++)
		{
			TGString string = (TGString)strings.get(i);
			if(value >= string.getValue())
			{
				boolean emptyString = true;
				
				if(beat != null)
				{
					Iterator it = beat.getNotes().iterator();
					while (it.hasNext()) 
					{
						TGNoteImpl note = (TGNoteImpl) it.next();
						if (note.getString() == string.getNumber()) 
						{
							emptyString = false;
							break;
						}
					}
				}
				if(emptyString){
					TGSongManager manager = TuxGuitar.instance().getSongManager();
					
					//comienza el undoable
					UndoableMeasureGeneric undoable = UndoableMeasureGeneric.startUndo();
					
					// Cria a nota
					TGNote note = manager.getFactory().newNote();
					note.setValue((value - string.getValue()));
					note.setVelocity(caret.getVelocity());
					note.setString(string.getNumber());
					note.setUser(user);
					
					// Seta quem foi o criador!
					
					TGDuration duration = manager.getFactory().newDuration();
					caret.getDuration().copy(duration);
					
					// System.out.println("Inserindo nota pelo piano!");
					// System.out.println("Value:" + String.valueOf(value));
					// System.out.println("String:" + string.getValue());
					// System.out.println("Caret.getMeasue():" + caret.getMeasure());
					// System.out.println("Caret.getPosition():" + caret.getPosition());
					// System.out.println("note:" + note);
					// System.out.println("duration:" + duration);
					
					manager.getMeasureManager().addNote(caret.getMeasure(),caret.getPosition(),note,duration);
					
					//termia el undoable
					TuxGuitar.instance().getUndoableManager().addEdit(undoable.endUndo());
					TuxGuitar.instance().getFileHistory().setUnsavedFile();
					
					//reprodusco las notas en el pulso
					// caret.getSelectedBeat().play();
					
					return true;
				}
			}
		}
		
		return false;


    	
	}
    
	protected void MudaTrack(String u,Caret c) 
	{
		int n_track = 0;
		
		TGTrack track = TuxGuitar.instance().getSongManager().getFirstTrack();
		if(track != null)
		{
			if( u.startsWith("A") )
				n_track = track.getNumber();
	   		if( u.startsWith("B") )
	   			n_track = track.getNumber() + 1;
	   		if( u.startsWith("C") )
	   			n_track = track.getNumber() + 2;
	   		if( u.startsWith("D") )
	   			n_track = track.getNumber() + 3;
	   			
	   		c.update(n_track);		
		}
				
	}
    
	protected void afterAction() 
	{
		int measure = TuxGuitar.instance().getTablatureEditor().getTablature().getCaret().getMeasure().getNumber();
		
		TuxGuitar.instance().getTablatureEditor().getTablature().getViewLayout().fireUpdate(measure);
		TuxGuitar.instance().updateCache(true);
	}

	protected void MoveDireita() 
	{
		if(TuxGuitar.instance().getPlayer().isRunning())
		{
			TuxGuitar.instance().getTransport().gotoNext();
		}
		else
		{
			
			/*GoRightAction d = TuxGuitar.instance().getAction(GoRightAction.NAME);
			
			TuxGuitar.instance().getAction(GoRightAction.NAME) */
			
			Caret caret = TuxGuitar.instance().getTablatureEditor().getTablature().getCaret();
			
			// Acertar
			// System.out.println("Movendo-se para a direita !!");
			if(!caret.moveRight())
			{
			 	int number = (TuxGuitar.instance().getSongManager().getSong().countMeasureHeaders() + 1);
			
				//comienza el undoable
				UndoableAddMeasure undoable = UndoableAddMeasure.startUndo(number);
				
				// System.out.println("Movendo-se para a direita e criando compasso!!");
				
				TuxGuitar.instance().getSongManager().addNewMeasure(number);
				TuxGuitar.instance().getTablatureEditor().getTablature().getViewLayout().fireUpdate(number);
	
				// Por algum motivo esse caret gera algum problema e a nota não é apresentada
				caret.moveRight();
				
				TuxGuitar.instance().getFileHistory().setUnsavedFile();
				
				//termia el undoable
				TuxGuitar.instance().getUndoableManager().addEdit(undoable.endUndo());
			
			}
		}
	
	}
    
    public int getValue (String nota)  
    {
    	int ret = 0;
    	
    	
    	// Teclas normais (brancas)
    	if(nota.equals("C1.WAV"))
    		ret = 48;
    	if(nota.equals("D1.WAV"))
    		ret = 50;
    	if(nota.equals("E1.WAV"))
    		ret = 52;
    	if(nota.equals("F1.WAV"))
    		ret = 53;
    	if(nota.equals("G1.WAV"))
    		ret = 55;
    	if(nota.equals("A1.WAV"))
    		ret = 57;
    	if(nota.equals("B1.WAV"))
    		ret = 59;
    	if(nota.equals("C2.WAV"))
    		ret = 60;
    	if(nota.equals("D2.WAV"))
    		ret = 62;
    	if(nota.equals("E2.WAV"))
    		ret = 64;
    	if(nota.equals("F2.WAV"))
    		ret = 65;
    	if(nota.equals("G2.WAV"))
    		ret = 67;
    	if(nota.equals("A2.WAV"))
    		ret = 69;
    	if(nota.equals("B2.WAV"))
    		ret = 71;
    	if(nota.equals("C3.WAV"))
    		ret = 72;
    		
    	// Teclas sustenidos (pretas)
    	
    	if(nota.equals("CS1.WAV"))
    		ret = 49;
    	if(nota.equals("DS1.WAV"))
    		ret = 51;
    	if(nota.equals("FS1.WAV"))
    		ret = 54;
    	if(nota.equals("GS1.WAV"))
    		ret = 56;
    	if(nota.equals("AS1.WAV"))
    		ret = 58;
    	if(nota.equals("CS2.WAV"))
    		ret = 61;
    	if(nota.equals("DS2.WAV"))
    		ret = 63;
    	if(nota.equals("FS2.WAV"))
    		ret = 66;
    	if(nota.equals("GS2.WAV"))
    		ret = 68;
    	if(nota.equals("AS2.WAV"))
    		ret = 70;
        
        return ret;
    }

}

